<h1>Goal</h1>
Save messages from multiple websocket conncetions into database, able to query them using different parameters.

Improving on writing clean code using Uncle Bob's clean code principles. Clean Architechture. Learning and applying solid and design patterns.

<h3>Producer/Consumer services</h3>
Producer-Consumer webservices using .NET worker services and RabbitMQ as queue. Each service should be independant and able to scale.
Producer service - listens to multiple Pusher websocket channels, based on different message handling strategies, writes messages into RabbitMQ queue. 
Consumer service - consumes messages from RabbitMQ queue, parses them and saves into PostgreSQL database. Using batch inserts to database to improove performance.
Data normalization.

React frontend
Ability to query logs based on channel, sender, message, timestamp other parameters.

Use metrics -  Prometheus and or Grafana


