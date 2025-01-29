using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentFTP;
using Microservices.Shared.Mocks;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;

namespace Microservices.Shared.CloudFiles.Ftp.IntegrationTests;

internal class FtpFilesTestsContext : IDisposable
{
    private readonly IContainer _container;
    private readonly FtpFilesOptions _options;
    private readonly IOptions<FtpFilesOptions> _mockOptions;
    private readonly AsyncFtpClient _asyncFtpClient;
    private readonly MockLogger<FtpFiles> _mockLogger;
    private bool _disposedValue;

    internal FtpFiles Sut => new(_mockOptions, _asyncFtpClient, _mockLogger);

    internal FtpFilesTestsContext()
    {
        var builder = new ContainerBuilder()
            .WithImage("ubuntu/nginx:1.26-24.10_edge") // nginx is just a convenient image to use to get an ubuntu container to work with
            .WithName($"FtpFilesTests.FTP_{Guid.NewGuid():N}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .WithPortBinding(10020, 20)
            .WithPortBinding(10021, 21);
        foreach (var pasvPort in Enumerable.Range(10200, 100))
            builder = builder.WithPortBinding(pasvPort, pasvPort);
;
        _container = builder.Build();
        PrepareContainer().GetAwaiter().GetResult();

        _options = new() { Host = "localhost", Port = _container.GetMappedPublicPort(21), BaseDir = "/files" };
        _mockOptions = Substitute.For<IOptions<FtpFilesOptions>>();
        _mockOptions.Value.Returns(_options);
        _asyncFtpClient = new();
        _mockLogger = new();
    }

    private async Task PrepareContainer()
    {
        var conf = Encoding.ASCII.GetBytes(
@"background=YES
allow_writeable_chroot=YES
anon_max_rate=6250000
anon_mkdir_write_enable=YES
anon_other_write_enable=YES
anon_root=/var/ftp
anon_umask=000
anon_upload_enable=YES
anonymous_enable=YES
connect_from_port_20=YES
dirmessage_enable=YES
ftpd_banner=Welcome to the integration test FTP Server
listen=YES
log_ftp_protocol=YES
max_clients=100
max_login_fails=2
max_per_ip=${MAX_PER_IP}
pasv_max_port=10299
pasv_min_port=10200
seccomp_sandbox=NO
secure_chroot_dir=/var/run/vsftpd/empty
use_localtime=YES
write_enable=YES
xferlog_std_format=NO".Replace("\r\n", "\n"));

        await _container.StartAsync();
        await RunCommand("apt-get update");
        await RunCommand("apt-get install -y --no-install-recommends vsftpd");
        await RunCommand("apt-get clean");
        await RunCommand("rm -rf /var/lib/apt/lists");
        await RunCommand("mkdir -p /var/run/vsftpd/empty");
        await RunCommand("mkdir -p /etc/vsftpd");
        await RunCommand("mkdir -p /var/ftp/files");
        await RunCommand("mv /etc/vsftpd.conf /etc/vsftpd.orig");
        await RunCommand("chmod 777 /var/ftp/files");
        await RunCommand("chown ftp:ftp /var/ftp/files");

        await _container.CopyAsync(conf, "/etc/vsftpd.conf");
        await RunCommand("/usr/sbin/vsftpd");
    }

    private async Task RunCommand(params string[] args)
    {
        args = args.SelectMany(arg => arg.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        Trace.WriteLine($"> {string.Join(" ", args)}");
        var result = await _container.ExecAsync(args);
        Trace.WriteLine($"[{result.ExitCode}] {result.Stdout}");
        if (!string.IsNullOrWhiteSpace(result.Stderr))
            Trace.WriteLine($"ERROR: {result.Stderr}");        
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _asyncFtpClient?.Dispose();
            _container.DisposeAsync().GetAwaiter().GetResult();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
