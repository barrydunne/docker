#!/bin/bash

# Use retries to allow time for redis to become available
max_retries=10
retry_delay=3
retry_count=0
echo "Testing Redis server connection..."
while [ $retry_count -lt $max_retries ]; do

    if [ "$(redis-cli -h redis ping)" = "PONG" ]; then
        echo "Redis server is running."
        break;
    fi
    echo "Redis server is not running."
    ((retry_count++))
    sleep $retry_delay

done

if [ $retry_count -eq $max_retries ]; then
    echo "Maximum number of retries reached."
    echo "Failed to initialise redis data."
    exit 1
fi

echo Creating initial data

redis-cli -h redis SET microservices.infrastructure.secrets "{\"rabbit.user\":\"admin\",\"rabbit.password\":\"P@ssw0rd\",\"rabbit.vhost\":\"microservices\"}"
redis-cli -h redis SET microservices.publicapi.secrets "{\"mongo.connectionstring\":\"mongodb://admin:P%40ssw0rd@mongo.microservices-publicapi:27017?retryWrites=true&directConnection=true&authSource=admin\"}"
redis-cli -h redis SET microservices.state.secrets "{\"mongo.connectionstring\":\"mongodb://admin:P%40ssw0rd@mongo.microservices-state:27017?retryWrites=true&directConnection=true&authSource=admin\"}"
redis-cli -h redis SET microservices.email.secrets "{\"mysql.connectionstring\":\"Server=mysql.microservices-email;Database=email;Uid=admin;Pwd=P@ssw0rd;\"}"
