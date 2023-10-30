FROM rabbitmq:3.12.7-management
COPY start-cluster.sh /opt/rabbitmq/start-cluster.sh
RUN chmod +x /opt/rabbitmq/start-cluster.sh
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -yq curl