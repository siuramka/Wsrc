<h1>Goal</h1>
Save Websocket logs from different connections from a streaming website chat logs.


<h3>Producer/Consumer services</h3> - [WIP] </br>
Producer Service</br>
- Listens to multiple Pusher WebSocket connections, processes and writes to RabbitMQ queue.
Consumer Service</br>
- Parses consumed messages, saves to database accordingly.

Each service is containerized and can run independently. 

</br>

Web frontend</br> - [ ]

Web backend</br> - [ ]

Ability to query logs based on channel, sender, message, timestamp other parameters.</br> - [ ]

Use metrics -  Prometheus and or Grafana</br> - [ ]


# Architecture

## Technologies

### Worker Services
- **.NET Core 9**
- **.NET Worker Services**
- **Entity Framework**
- **PostgreSQL**
- **RabbitMQ**
- **Docker**
- **NUnit**
- **TestContainers**
- **FluentAssertions**
- **NSubstitute**

### API
- **.NET Core 9**
- **Minimal API**




