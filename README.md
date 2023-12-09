# Для запуска проекта необходимо:
### 1. собрать образы каждого сервиса, пока так:(
```
    docker build -t message-generator -f HexgazeP.RabbitMQMessageGenerator/Dockerfile .
    docker build -t message-consumer -f HexgazeP.Consumer/Dockerfile .
    docker build -t message-aggregator -f HexgazeP.Aggregator/Dockerfile .
    docker build -t message-api -f HexgazeP.API/Dockerfile .
    docker build -t message-batchqueue -f HexgazeP.BatchQueue/Dockerfile .
    docker-compose up -d
    
```
### 2. в реббите зайти в queue и добавить бинд с названием и роутингм "Message"