FROM ubuntu:trusty

RUN apt-get update \
    && apt-get install -y --no-install-recommends vsftpd \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Anonymous FTP access must upload into /files/ directory

RUN mkdir -p /var/run/vsftpd/empty \
 && mkdir -p /etc/vsftpd \
 && mkdir -p /var/ftp/files \
 && mv /etc/vsftpd.conf /etc/vsftpd.orig \
 && chmod 777 /var/ftp/files \
 && chown ftp:ftp /var/ftp/files

COPY init.sh /init.sh
RUN chmod +x /init.sh

ENTRYPOINT ["/init.sh"]
