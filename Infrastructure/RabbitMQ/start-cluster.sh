#!/bin/bash

if [ "$RABBITMQ_JOIN_NODE" = "" ]; then
    echo This is $RABBITMQ_NODENAME, running server
    rabbitmq-server
else
    echo This is $RABBITMQ_NODENAME, joining $RABBITMQ_JOIN_NODE

    # Start it in the background
    rabbitmq-server &
    pid=$!
    echo Started server with PID $pid

    # Wait for it to start
    echo Wait for RabbitMQ to be running
    while true; do
        rabbitmqctl status > /dev/null 2>&1
        exitCode=$?
        if [ $exitCode -eq 0 ]; then
            echo RabbitMQ is running
            break
        fi
        sleep 1
    done

    # Join cluster
    rabbitmqctl stop_app
    rabbitmqctl reset
    rabbitmqctl join_cluster $RABBITMQ_JOIN_NODE
    rabbitmqctl start_app

    # Ensure high availability policy is created
    credentials="${RABBITMQ_DEFAULT_USER}:${RABBITMQ_DEFAULT_PASS}"
    base64_credentials=$(echo -n "$credentials" | base64)
    echo Creating HighAvailability policy
    curl -X PUT \
         -H "Content-Type: application/json" \
         -H "Authorization: Basic ${base64_credentials}" \
         -d '{"vhost":"microservices","name":"HighAvailability","pattern":".*","apply-to":"queues","definition":{"ha-mode":"all","ha-sync-mode":"automatic"}}' \
         http://localhost:15672/api/operator-policies/microservices/HighAvailability

    # Resume running server
    if kill -0 $pid > /dev/null 2>&1; then
        echo "Background job with PID $pid is still running."
        wait $pid
    else
        echo "Background job with PID $pid has completed."
        rabbitmq-server
    fi

fi