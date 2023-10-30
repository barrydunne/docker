#!/bin/bash

set -e

ANON_ROOT=${ANON_ROOT:-/var/ftp}
MAX_PORT=${MAX_PORT:-10299}
MIN_PORT=${MIN_PORT:-10200}
MAX_PER_IP=${MAX_PER_IP:-200}
MAX_LOGIN_FAILS=${MAX_LOGIN_FAILS:-2}
MAX_CLIENTS=${MAX_CLIENTS:-100}
MAX_RATE=${MAX_RATE:-6250000}
FTPD_BANNER=${FTPD_BANNER:-"Welcome to anonymous FTP Server"}


[ -f /etc/vsftpd.conf ] || cat <<EOF > /etc/vsftpd.conf
allow_writeable_chroot=YES
anon_max_rate=${MAX_RATE}
anon_mkdir_write_enable=YES
anon_other_write_enable=YES
anon_root=${ANON_ROOT}
anon_umask=000
anon_upload_enable=YES
anonymous_enable=YES
connect_from_port_20=YES
dirmessage_enable=YES
ftpd_banner=${FTPD_BANNER}
listen=YES
log_ftp_protocol=YES
max_clients=${MAX_CLIENTS}
max_login_fails=${MAX_LOGIN_FAILS}
max_per_ip=${MAX_PER_IP}
pasv_max_port=${MAX_PORT}
pasv_min_port=${MIN_PORT}
seccomp_sandbox=NO
secure_chroot_dir=/var/run/vsftpd/empty
use_localtime=YES
write_enable=YES
xferlog_std_format=NO
EOF

/usr/sbin/vsftpd "$@"