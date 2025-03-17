workspace "DoLS Enron Dataset Assignment" "C4 model of the project" {

    !identifiers hierarchical

    model {
        user = person "User" {
            description "A user of the application."
        }

        data = softwareSystem "Emails In Folder" {
            description "The enron dataset"
        }

        enronSearchSystem = softwareSystem "DoLS System" {
            description "Processes and indexes dataset to allow searching"

	
	rabbitMq = container "Message system" "RabbitMQ"{
	  description "Messaging system between microservices"
                technology "RabbitMQ"
}
            
            postgresDatabase = container "Postgres Database" "PostgreSQL" {
                description "Stores clean emails"
                technology "PostgreSQL"
                tags "Database"
            }


            cleaner = container "Cleaner Service" ".NET Service" {
                description "Cleans emails to remove unwanted headers."
                technology ".NET Service"
                
                emailCleanerService = component "EmailCleanerService" {
                    description "Cleans email files"
                    technology "C# Service"
                }

                messagePublisher = component "MessagePublisher" {
                    description "Publishes cleaned files to RabbitMQ."
                    technology "C# Service"
                }
            }

            indexer = container "Indexer Service" ".NET Service" {
                description "Indexes clean emails"
                technology ".NET Service"
                
                emailIndexerService = component "EmailIndexerService" {
                    description "Processes clean emails"
                    technology "C# Service"
                }

                indexerRepository = component "IndexerRepository" {
                    description "Stores indexed files in the database"
                    technology "PostgreSQL"
                }

                messageHandler = component "MessageHandler" {
                    description "Takes incoming emails from the RabbitMQ Queue"
                    technology "C# Service"
                }

            }

            
            webApi = container "WebApi" "ASP.NET Core Web API" {
                description "Searches the database for user input"
                technology "ASP.NET Core Web API"
                
                searchController = component "SearchController" {
                    description "Recieves requests and forwards data"
                    technology "C# Service"
                }

                searchService = component "SearchService" {
                    description "Passes data to repository"
                    technology "C# Service"
                }

                searchRepository = component "SearchRepository" {
                    description "Searches Database for user input"
                    technology "PostgreSQL"
                }

            }


            frontend = container "Frontend" {
                description "Frontend that takes user input and forwards it to the search api"
                technology "React and Tailwind"
            }


            user -> frontend "searches for term"
            frontend -> webApi "sends query and waits for response"
            webApi -> postgresDatabase "searches database"
            indexer.messageHandler -> rabbitMq "Gets cleaned files"
            indexer.emailIndexerService -> postgresDatabase "inserts files into database"
            cleaner.emailCleanerService -> data "cleans files"
		cleaner.messagePublisher -> rabbitMq "Publishes clean files"
            
        }
    }

    views {
        systemContext enronSearchSystem system_context {
            include *
            autolayout lr
        }

        container enronSearchSystem container_diagram {
            include *
            autolayout tb

        }

        component enronSearchSystem.cleaner cleaner_component_diagram {
            include *
            autolayout lr

        }

        component enronSearchSystem.indexer indexer_component_diagram {
            include *
            autolayout lr

        }

        component enronSearchSystem.webApi webapi_component_diagram {
            include *
            autolayout lr

        }

        
        theme default
    }
}