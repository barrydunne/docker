# Introduction

The code in this repository demonstrates running a full microservice, message based, distributed application within docker containers on a local development environment.
This is a purely personal project with the sole purpose of allowing me to work on technologies I am interested in.
It was developed entirely from scratch by me working alone in my spare time.

## Premise

The premise of this fictional application is to have an API that accepts a starting address, a destination address and an email. The application will then send an email with a journey report.

The API will immediately return a `JobId`. The following tasks are then performed asynchronously and concurrently by independent services:

  - Perform geocoding on the two addresses to get latitude and longitude.
  - Produce an image of the destination.
  - Produce a weather forecast for the destination.
  - Produce directions for the journey from the starting location to the destination.
  - Send a notification email when the job is complete containing the details.

*Note that the geocoding, directions, weather and imaging services are set to use dummy data by default.
To use real external services API keys are required and the services must be enabled in their respective Api's IoC class.*

## Quickstart

  - Have Docker installed and running, have .net 8 SDK installed.
  - Run `./Scripts/build-all.ps1` to build and run all services followed by a full synthetic end-to-end test that posts a new request and verifies the final email is received.
  - Use the API at http://localhost:11080/ to post requests. (_An endpoint is available to generate authentication tokens required to make requests_)
  - Check the notification emails at http://localhost:17080 (_Emails are caught here and not sent publicly_)
  - View logs at http://localhost:10081/#/events?range=1d&signal=signal-19&tail
  - View the metrics at http://localhost:10088/dashboards

## Sample

The following is a sample request to the API (_Authorization and X-Idempotency-Key headers are not shown_):
```
{
  "startingAddress": "Silver Creek Court, Orlando, FL 34714, United States",
  "destinationAddress": "Chocolate Emporium, 6000 Universal Blvd, Orlando, FL 32819, United States",
  "email": "microservices.notifications@example.com"
}
```
This request will result in the following email being sent:

<a href="Screenshots/EmailReport.png" target="_blank"><sup>Click for full size image</sup><img src="Screenshots/EmailReport.png" width="200px"/></a>

The example shown was captured when running with real external services.

## Features

  - Over 70 custom metrics captured and visible in Grafana for the individual services as well as metrics for RabbitMQ.

    <a href="Screenshots/MetricsPublicApi.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/MetricsPublicApi.png" width="100px"/></a>
    <a href="Screenshots/MetricsState.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/MetricsState.png" width="100px"/></a>
    <a href="Screenshots/MetricsImaging.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/MetricsImaging.png" width="100px"/></a>
    <a href="Screenshots/MetricsRabbitMQ.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/MetricsRabbitMQ.png" width="100px"/></a>

  - Over 500 tests (unit and integration) providing 100% code coverage using NSubstitute ~~Moq~~, AutoFixture and Shouldly. Integration tests using TestContainers for external dependencies, including a RabbitMQ cluster.

    <a href="Screenshots/CodeCoverage.png" target="_blank"><img src="Screenshots/CodeCoverageHeader.png" width="400px"/></a>

  - 27 containers, including a RabbitMQ cluster with 2 nodes as well as local 'cloud' services make up the running application.

    <a href="Screenshots/ContainersInfrastructure.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/ContainersInfrastructure.png" width="200px"/></a>
    <a href="Screenshots/ContainersServices.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/ContainersServices.png" width="200px"/></a>

  - Fluent validation, MediatR pipeline validation, and guard clauses are used to protect against bad input.

    <a href="Screenshots/FluentValidation.png" target="_blank"><img src="Screenshots/FluentValidation.png" width="400px"/></a>

  - Tracing is implemented automatically for command handlers and other processing.

    <a href="Screenshots/Trace.png" target="_blank" style="display:inline-block;vertical-align:top;"><img src="Screenshots/Trace.png" width="400px"/></a>

  - Caching is used at different levels and is implemented with:
      - **nginx** - caches the responses for requests using the same idempotency key to prevent unnecessary round trips to the API.
      - **in-memory** - the public API uses an in-memory job status cache to prevent unnecessary round trips to the database.
      - **redis** - the geocoding service caches address/coordinates key value pairs in redis with limited time-to-live to prevent unnecessary, and potentially costly, round trips to the external service.

  - Cloud provider can be switched by simply changing the `Microservices.CloudProvider` environment variable.

## Technology showcase

This codebase showcases the following technologies:

• Asp.Net Core 8
• AutoFixture
• AWS
• Cloud services
• Code coverage
• CQRS
• DevOps
• Docker
• EntityFramework
• Github workflows
• Grafana
• Guard clauses
• Idempotency
• IdentityServer4
• LocalStack
• Mapster (auto mapping)
• Markdown
• MediatR
• Message queues
• Metrics
• Microservices
• Minimal API
• Mocks
• MongoDB
• ~~Moq~~
• MySQL
• Nginx
• NSubstitute
• NuGet
• NUnit
• OAuth2
• OpenTelemetry
• Polly
• PowerShell
• Prometheus
• RabbitMQ
• Redis
• REST
• Seq
• Serilog
• Shouldly
• Structured logging
• Swagger
• TestContainers
• Tests (unit/integration/end-to-end/fluent contexts)

## Technical requirements:

  - The public API will require JWT bearer token authentication.
  - The original API request will require an idempotency key in the request header. If the same idempotency key is reused within 7 days then the same job id will be returned and no new job will be processed. After 7 days it will be treated as a new request and will be processed.
  - The system should be resilient to a failure of one of the RabbitMQ nodes and continue operations using the alternate node.
  - Any message that cannot be handled due to an exception in the handler will be retried after a 30 second delay. Retries will occur indefinitely with no expiry.
  - A second API endpoint will be available that accepts the job id and returns the status (Accepted, Processing, Failed (with reason), Complete)
  - The directions, image and weather tasks are all dependent on the geocoding task which must complete first. Once geocoding is complete the other tasks will run in parallel.
  - There will be no direct communication between the services, or knowledge of each other. Services may only connect to the shared cloud infrastructure.
  - All external services used (geocoding, directions, weather, imaging) will be injected using interfaces and dummy implementations will be available to avoid the requirement for external API keys when running the application.
  - All cloud services used (message queues, secrets, file storage, email) will be injected using interfaces. Docker based implementations will be available to avoid the requirement for AWS or Azure accounts when running the application.
  - As a development aid, all services should provide HTTP API endpoints to allow developers to post events. This is in addition to the normal ingestion method of subscribing to published events. Whether the event is received via message or HTTP request the same processing will be performed. It is not required to return any content in the response to the HTTP requests.
  - Centralised logging and metrics are required detailing durations and counters for every operation.
  - All logging related to a job, from any component, will include the job id as a correlation id to allow tracing of all activity for a single job
  - There will be an API with pagination support to allow searching for the times and recipients of sent emails.

## Development requirements

  - All C# projects will use .net 8.
  - All C# projects should build with no code warnings.
  - The services must follow clean architecture principles with API separate and loosely coupled to the application logic using MediatR.
  - All logic code must have tests that require 100% code coverage threshold for line, branch and method coverage.
  - All non-private production classes and methods must have XML documentation.
  - Tests and code coverage requirements must pass as part of building docker images.
  - All custom Docker images should include health checks.
  - All API should provide a /health/version endpoint that returns the timestamp when the service was built.
  - All powershell scripts, all Docker image builds, all running containers and all tests must work on both Windows and Linux hosts.

## Not required

  - Job ownership - any client will be able to get the job status of any job.
  - Resending email notifications.
  - Message sequence checks - it can be assumed in this simple application that all messages are received in the correct sequence regardless of how many consumer instances are running. No checks are required to ensure message processing sequence.

## Components

The application consists of the following components:

  - **Microservices.SharedLibraries** - A set of nuget packages providing cross-cutting functionality used by the application.
  - **Infrastructure** - Provides the 'cloud' network and services used by the application, such as messaging, logging, file storage, email server.
  - **Public API** - The initial API to create jobs and request job status.
  - **Geocoding** - The service to geocode addresses into coordinates.
  - **Directions** - The service to provide directions for the journey.
  - **Weather** - The service to provide a weather forecast for the destination.
  - **Imaging** - The service to provide an image of the destination.
  - **Email Notification** - The service to send email notifications when processing is complete.
  - **State Manager** - Monitors and maintains the state of the job, triggering the notification when complete.

## Events
The sequence of events during processing:

```mermaid
sequenceDiagram
    participant API as Public API
    box
    participant MQ as Message Bus
    end
    participant State as State Service
    participant Geo as Geocoding Service
    box Parallel Processing
    participant D as Directions Service
    participant W as Weather Service
    participant I as Imaging Service
    end
    participant Email as Email Notification Service

    API--)+MQ: JobCreated
    MQ--)+Geo: JobCreated
    MQ--)-State: JobCreated
    activate State
    State--)+MQ: JobStatusUpdate(Processing)
    deactivate State
    MQ--)-API: JobStatusUpdate(Processing)
    alt
        Geo--)+MQ: GeocodingComplete(Success)
        deactivate Geo
        MQ--)-State: GeocodingComplete(Success)
        activate State
        State--)MQ: LocationsReady
        deactivate State
        par
        activate MQ
        MQ--)D: LocationsReady
        activate D
        D--)+MQ: DirectionsComplete
        deactivate D
        MQ--)-State: DirectionsComplete
        and
        MQ--)W: LocationsReady
        activate W
        W--)+MQ: WeatherComplete
        deactivate W
        MQ--)-State: WeatherComplete
        and
        MQ--)I: LocationsReady
        activate I
        I--)+MQ: ImagingComplete
        deactivate I
        MQ--)-State: ImagingComplete
        deactivate MQ
        activate State
        end
        State--)+MQ: JobStatusUpdate(Complete)
        MQ--)-API: JobStatusUpdate(Complete)
        State--)+MQ: ProcessingComplete(Success)
        deactivate State
        MQ--)-Email: ProcessingComplete(Success)
    else
        Geo--)+MQ: GeocodingComplete(Failure)
        MQ--)-State: GeocodingComplete(Failure)
        activate State
        State--)+MQ: JobStatusUpdate(Failed)
        MQ--)-API: JobStatusUpdate(Failed)
        State--)+MQ: ProcessingComplete(Failure)
        deactivate State
        MQ--)-Email: ProcessingComplete(Failure)
    end
```

## Microservices.SharedLibraries
The `Microservices.SharedLibraries.sln` solution contains the following projects, along with unit and integration tests.
  - **Microservices.Shared.CloudEmail** - Contains the interface(s) for sending email.
  - **Microservices.Shared.CloudEmail.Aws** - Contains the implementation to send email using the AWS SES service.
  - **Microservices.Shared.CloudEmail.Smtp** - Contains the implementation to send email using the SMTP service in the infrastructure.
  - **Microservices.Shared.CloudFiles** - Contains the interface(s) for storing files.
  - **Microservices.Shared.CloudFiles.Aws** - Contains the implementation to store files using the AWS S3 service.
  - **Microservices.Shared.CloudFiles.Ftp** - Contains the implementation to store files using the FTP service in the infrastructure.
  - **Microservices.Shared.CloudSecrets** - Contains the interface(s) for retrieving secrets from a secure vault. It also provides extensions to allow secrets to be applied to configuration values from appsettings.json at startup.
  - **Microservices.Shared.CloudSecrets.Aws** - Contains the implementation to retrieve secrets from the AWS SecretsManager service.
  - **Microservices.Shared.CloudSecrets.SecretsManager** - Contains the implementation to retrieve secrets from the secrets manager API service in the infrastructure.
  - **Microservices.Shared.Events** - Contains the event types used throughout the application
  - **Microservices.Shared.Mocks** - Contains shared mocks that may be used in individual test projects.
  - **Microservices.Shared.Queues** - Contains the interface(s) for publishing and subscribing to events.
  - **Microservices.Shared.Queues.RabbitMQ** - Contains the implementation to publish and subscribe to events using the RabbitMQ service in the infrastructure.
  - **Microservices.Shared.RestSharpFactory** - Provides the interface and default implementation to create IRestClient instances. This allows test projects to use a mock factory to produce mock IRestClient instances.
  - **Microservices.Shared.Utilities** - Provides common code and extension methods.

To create packages:

```
cd path\to\Microservices.SharedLibraries
.\build.ps1
```

The packages will be available when building the service containers as a local package source that is added during build.
They are not published to nuget.org or any other package repository.

A local package source may be added on the development computer to allow building the services outside of containers with the following command:
```
dotnet nuget add source path\to\Microservices.SharedLibraries\Packages -n Microservices.SharedLibraries
```

The `build.ps1` script will clear the local nuget cache so that the latest versions will be used locally whenever a consuming project is rebuilt.

## Infrastructure
This creates the cross-cutting services used throughout the application.
It is a prerequisite for all other services and includes:

  - Shared network using IPs in the subnet `172.30.0.0/24`
  - Message bus - **[RabbitMQ](https://www.rabbitmq.com/)**
  - OpenID Connect and OAuth 2.0 server - **[IdentityServer4](https://identityserver4.readthedocs.io/en/latest/)**
  - Consolidated logging - **[Seq](https://datalust.co/)**
  - Consolidated metrics capture - **[Prometheus](https://prometheus.io/)**
  - Consolidated metrics visualisation - **[Grafana](https://grafana.com/)** with preconfigured dashboards for RabbitMQ, Redis and other services
  - AWS services - ** [LocalStack](https://www.localstack.cloud/)**
  - 'Cloud' file storage - **VSFTP**
  - 'Cloud' secrets manager - **.Net 8 API** using **[Redis](https://redis.io/)** for persistence
  - 'Cloud' Email service - **[MailCatcher](https://mailcatcher.me/)**

These services will use the `172.30.0.02 - 172.30.0.19` IP range in the shared network and the `10xxx` port range on the host.

To create:

Run `./Scripts/build-infrastructure.ps1` or
```
cd path\to\Infrastructure\DockerCompose
docker compose -p microservices-infrastructure up -d --build
```

## Public API
This is the public entrypoint to the application
  - Nginx - Used for idempotency key handling
    - Enforce idempotency key header requirement
    - Return cached responses for the same idempotency key without hitting the backend
  - .Net 8 API - Used to initiate a distributed workflow
  - MongoDB - Used for data persistence

These services will use the `172.30.0.20 - 172.30.0.29` IP range in the shared network and the `11xxx` port range on the host.

To create:

Run `./Scripts/build-publicapi.ps1` or
```
cd path\to\PublicAPI\DockerCompose
docker compose -p microservices-publicapi up -d --build
```


## State
This is the service responsible for maintaining the state of a job, including the status and data.
  - .Net 8 API - Used to maintain state
  - MongoDB - Used for data persistence

These services will use the `172.30.0.30 - 172.30.0.39` IP range in the shared network and the `12xxx` port range on the host.

To create:

Run `./Scripts/build-state.ps1` or
```
cd path\to\State\DockerCompose
docker compose -p microservices-state up -d --build
```


## Geocoding
This is the service responsible for geocoding locations
  - .Net 8 API - Used to perform geocoding
  - Redis - Used for caching of coordinates from external service

These services will use the `172.30.0.40 - 172.30.0.49` IP range in the shared network and the `13xxx` port range on the host.

To create:

Run `./Scripts/build-geocoding.ps1` or
```
cd path\to\Geocoding\DockerCompose
docker compose -p microservices-geocoding up -d --build
```


## Directions
This is the service responsible for producing journey directions.
  - .Net 8 API - Used to generate directions

These services will use the `172.30.0.50 - 172.30.0.59` IP range in the shared network and the `14xxx` port range on the host.

To create:

Run `./Scripts/build-directions.ps1` or
```
cd path\to\Directions\DockerCompose
docker compose -p microservices-directions up -d --build
```


## Weather
This is the service responsible for producing weather forecasts.
  - .Net 8 API - Used to generate weather forecasts

These services will use the `172.30.0.60 - 172.30.0.69` IP range in the shared network and the `15xxx` port range on the host.

To create:

Run `./Scripts/build-weather.ps1` or
```
cd path\to\Weather\DockerCompose
docker compose -p microservices-weather up -d --build
```


## Imaging
This is the service responsible for producing images of the destination.
  - .Net 8 API - Used to generate images

These services will use the `172.30.0.70 - 172.30.0.79` IP range in the shared network and the `16xxx` port range on the host.

To create:

Run `./Scripts/build-imaging.ps1` or
```
cd path\to\Imaging\DockerCompose
docker compose -p microservices-imaging up -d --build
```


## Email
This is the service responsible for sending email notifications.
  - .Net 8 API - Used to send emails

These services will use the `172.30.0.80 - 172.30.0.89` IP range in the shared network and the `17xxx` port range on the host.

To create:

Run `./Scripts/build-email.ps1` or
```
cd path\to\Email\DockerCompose
docker compose -p microservices-email up -d --build
```


## IP / Port Reservations

The bold entries are for the main services of interest to use and monitor the application.

API - http://localhost:11080
Logs - http://localhost:10081
Metrics - http://localhost:10088
Aspire Dashboard - http://localhost:10089

\* _Local port mappings for these services are made available for diagnostics purposes in this development environment._

| IP Address  | Local Port(s) | Project        | Service              | Link(s) |
| ------------| -------------- | -------------- | -------------------- | ------- |
| 172.30.0.2  | 10672*, 10692* | Infrastructure | RabbitMQ node 1      | http://localhost:10672 (admin / P@ssw0rd) http://localhost:10692/metrics |
| 172.30.0.3  | 10673*, 10693* | Infrastructure | RabbitMQ node 2      | http://localhost:10673 (admin / P@ssw0rd) http://localhost:10693/metrics |
| 172.30.0.4  | 10081          | Infrastructure | **Seq**              | http://localhost:10081/#/events?range=1d&signal=signal-19&tail |
| 172.30.0.5  |                | Infrastructure | Seq Init             |  |
| 172.30.0.6  | 10082*         | Infrastructure | Prometheus           | http://localhost:10082 |
| 172.30.0.7  | 10088          | Infrastructure | **Grafana**          | http://localhost:10088/d/publicapi/public-api?orgId=1&refresh=5s&from=now-1h&to=now |
| 172.30.0.8  | 10020*, 10021*, 10100-10199* | Infrastructure | VSFTP  | ftp://localhost:10021  |
| 172.30.0.9  | 10379*         | Infrastructure | Redis                | redis-cli -h localhost -p 10379 |
| 172.30.0.10 |                | Infrastructure | Redis Init           |  |
| 172.30.0.11 | 10121*         | Infrastructure | Redis Metrics        | http://localhost:10121/metrics |
| 172.30.0.12 | 10083*         | Infrastructure | Secrets Manager      | http://localhost:10083 |
| 172.30.0.13 | 10025*, 10084  | Infrastructure | **MailCatcher**      | http://localhost:10084 |
| 172.30.0.14 | 10004*         | Infrastructure | IdentityServer4      | http://localhost:10004/.well-known/openid-configuration |
| 172.30.0.15 | 10089          | Infrastructure | **Aspire Dashboard** | http://localhost:10089 |
| 172.30.0.16 | 10566          | Infrastructure | LocalStack           |  |
| 172.30.0.20 | 11080          | Public API     | **Nginx**            | http://localhost:11080 |
| 172.30.0.21 | 11081*, 11082* | Public API     | Public API           | http://localhost:11081 http://localhost:11082/metrics |
| 172.30.0.22 | 11017*         | Public API     | MongoDB              | mongodb://admin:P%40ssw0rd@localhost:11017/
| 172.30.0.30 | 12080*, 12081* | State          | State API            | http://localhost:12080 http://localhost:12081/metrics |
| 172.30.0.31 | 12017*         | State          | MongoDB              | mongodb://admin:P%40ssw0rd@localhost:12017/
| 172.30.0.40 | 13080*, 13081* | Geocoding      | Geocoding API        | http://localhost:13080 http://localhost:13081/metrics |
| 172.30.0.41 | 13379*         | Geocoding      | Redis                | redis-cli -h localhost -p 13379 |
| 172.30.0.50 | 14080*, 14081* | Directions     | Directions API       | http://localhost:14080 http://localhost:14081/metrics |
| 172.30.0.60 | 15080*, 15081* | Weather        | Weather API          | http://localhost:15080 http://localhost:15081/metrics |
| 172.30.0.70 | 16080*, 16081* | Imaging        | Imaging API          | http://localhost:16080 http://localhost:16081/metrics |
| 172.30.0.80 | 17080*, 17081* | Email          | Email API            | http://localhost:17080 http://localhost:17081/metrics |
| 172.30.0.81 | 17306*         | Email          | MySQL                | Server=localhost;Port=17306;Database=email;Uid=admin;Pwd=P@ssw0rd;

