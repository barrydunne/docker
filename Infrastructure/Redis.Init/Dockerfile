FROM redis:7.2.2
COPY ./redis-initial-data.sh /data/redis-initial-data.sh
RUN chmod +x /data/redis-initial-data.sh
ENTRYPOINT ["/data/redis-initial-data.sh"]