# DoLSCompulsory
Development of Large Systems Compulsory Assignment

User Manual:

1. download the dataset and put it in the Data folder in the project root
2. Run the Data splitter script
3. create a dotenv file with the following variables and put it in the project root:
   # RabbitMq
    RABBITMQ_HOST= value
    RABBITMQ_PORT= value
    RABBITMQ_USER= value
    RABBITMQ_PASS= value

    # Postgres
    POSTGRES_HOST= value
    POSTGRES_PORT= value
    POSTGRES_USER= value
    POSTGRES_PASS= value
    POSTGRES_DB= value
4. Build and run the docker-compose file
