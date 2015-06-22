# Awesome Logger
Log monitoring and notification system. The system allows its users to monitor intranet application logs and receive notifications if specific events occur.

* [Architecture](#architecture)
* [Components](#components)
* [Requirements](#requirements)
* [Technologies](#technologies)
* [How to Build](#how-to-build)
* [How to Install](#how-to-install)
* [Further Improvements](#further-improvements)
* [License](#license)
* [Contacts](#contacts)

## Architecture
The system is built using Event-Driven-Architecture (EDA) and Service-Oriented-Architecture (SOA) principles. The system is breaked up into small micro-services, independent separated pieces with single responsibility. Each service owns its data (diffenerent data storages can be used for each micro-service), can be versioned, updated and deployed separateley and can be scaled up horizontally independently if needed. 

Because subscriptions data and notification data have a very different frequency and purpose, I've decided to split theses APIs into two micro-services: `Subscriptions API` and `Notification Service`.

The system is distributed and designed without Single-Point-Of-Failure (SPOF). Each component can be scaled and load balanced. For Audit DB NoSQL database is recommended because of high load and log-like structure (heavy write, no changes, low read). Cassandra can be perfectly meet this data usage pattern.

![](Assets/AwesomeLogger-Architecture.png?raw=true)


1. When a `Monitor Service` starts its makes a request to `Subscriptions API` to get subscription parameters for client machine. For security reasons `Subscriptions API` should use SSL to encrypt messages. Also, communication between client and API is protected with access token, which is specified in configuration files on both sides: for `Monitor Service` its an *AccessToken* in App.config, for `Subscriptions API` service its an *ExternalAccessToken* in Web.config.
2. `Subscription API` executes query agains `Subscriptions DB` to find any subscription parmaeters for client machine. If there are any API sends them to client machine and `Monitor Service` starts parsing logs.
3. If `Monitor Service` finds any match it emits special message *Pattern Match Found* to `Service Bus`. Connection between `Service Bus` and publishers/subscribers is encrypted and configured while installation process.
4. One or many `Notification Services` listening to *Pattern Match Found* messages compete for message processing. If there are a lot of messages generated you can simple install more `Notification Services`. 
5. When the service receives notification message, its first tries to make an audit record on the `Audit API` service. If the service failed to make an audit record it's return message to the `Service Bus` queue, so another `Notification Service` could be more lucky. 
6. If the reason of `Audit API` failure is duplicate conflict in `Audit DB`, the service just removes the message from the `Service Bus` and starts listening for other messages.
7. After successfull commitment of the audit record, the service sends notification using external email service. In our case it is an easy to use `SendGrid` service. You can specify username/password for your `SendGrid` account at the `App.config` of the `NotificationService`.
8. If an error occured while parsing/monitoring log files in `Monitor Service`, the service emits special type of event into the `Service Bus` so special service `Error-Handling Service` could process this error. Typically `Error-Handling Service` just logging error messages so an admin can view them later.
9. The system has a web-based user interface (Web UI or Website) to create/update/delete subscriptions. Only one user `System Administrator` can access this interface. `Username/password` settings for administrator account can be configured in `Web.config` of the `Web` project. For security reasons SSL should be used to encrypt data between Web UI and administrator PC.
10. When `System Administrator` successfully log on to the `Website`, the site proxying requests and using secret `AccessToken` making requests to `Subscription API`. 
11. Administrator can even view all notifications for each subscriptions from `AuditDB`. 
12. When Administrator creates new subscription or modify existing through `Web UI`, `Subscriptions API` detects those changes and emits special kind of *Update Subscription* event to `Service Bus`.
13. `Monitor Service` receives *Update Subscription* event and restarts itself by retrieving new configuration from `Subscriptions API`.

Using Service Bus as a communication channel makes the system robust and fault-tolerant. If one of the services fails, no messages will be lost and after recovering subscribers will receive all unprocessed messages.

## Components
The system consists of several services and website. All components are loosely coupled and can be deployed and upgraded independently. 

### Website
Web UI is implemented as a Single-Page-Application using Angular.js

#### Login page

![](Assets/screenshots/login.png?raw=true)

* Enter username/password for admin and click `Sign in`. Default credentials specified in Web.config: `admin/admin`

#### Subscriptions page

![](Assets/screenshots/subscriptions.png?raw=true)

* You can list all existing subscriptions
* Create new subscription
* Go to details page

#### Subscription details page

![](Assets/screenshots/subscription-details.png?raw=true)

* Change values and click `Save`. After that all logs will be re-scanned.
* You can discover monitoring history by clicking `View History` at the bottom.

#### Subscription history page

![](Assets/screenshots/subscription-history.png?raw=true)

* All found matches in reverse chronological order it was discovered.
* Hover mouse over `Time` field to view date also.
* Click `Refresh` button to reload page with newer results.

### Subscriptions API
Implemented as a RESTful API HTTP service. Unlike most of WCF bindings HTTP API is supported by major of clients. The service is implemented in micro-service architecture, has minimum dependencies and does not share it's data with other components.

This API is consumed by `Website` for managing subscriptions by Administrator and by `Monitor Service` to retrieve subscription params. For accessing this API you need AccessToken which is specified on configuration file. For Read-Write access there is a dedicated AccessToken using in Website->API communication. Fir Monitor->API communication there is a different AccessToken.

Address | HTTP Method | Description 
:--- | :--- | :---
/ | GET | Returns [queryable](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options) collection of subscriptions.
/ | POST | Creates new subscription.
/{id} | GET | Returns subscriptions by id.
/{id} | PUT | Updates subscriptions by id.
/{id} | DELETE | Deletes subscription by id.
/machine/{name} | GET | Retrieves all subscriptions by machine name.

### Audit API
Implemented as a RESTful API HTTP service. Unlike most of WCF bindings HTTP API is supported by major of clients. The service is implemented in micro-service architecture, has minimum dependencies and does not share it's data with other components.

This API is consumed internally by `Website` for displaying Subscription History and by `Notification Service` for creating audit records. For accessing this API you need AccessToken which is specified on configuration file.

Address | HTTP Method | Description 
:--- | :--- | :---
/ | POST | Records pattern-match event.
/?m={m}&s={s}&p={p}&e={e} | GET | Returns [queryable](http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options) collection of pattern matches. {m} - machine name, {s} - search path, {p} - pattern to match, {e} - email to send notifications to.

### Error-Handling Service
Implemented as a Console Application. Can also be [installed](#installing-error-handling-service) as a Windows Service. Listens for error messages and logs them into the Windows Event Log.

### Notification Service
Implemented as a Console Application. Should be [installed](#installing-notification-service) as a Windows Service.
Listens for notification messages then trying to create an audit record using `Audit API` and send email using third-party service `SendGrid`.

* If audit request failes due to duplicate conflict exception, message should be dropped from Service Bus.
* if audit request failes due to some other error, message should be returned to Service Bus. Another `NotificationService` instance will try to process it.
* If audit record committed, then service will try to send email (if SendGrid credentials specified in App.config).

### Monitoring Service
Implemented as a Console Application. Should be [installed](#installing-monitor-service) as a Windows Service.

* Retrieves related subscription parameters from `Subscriptions API` immediately after start.
* Listen for messages about subscription update. If related subscription updated, monitor stops all parsers and retrieves fresh subscription parameters from `Subscriptions API`.
* Scans files in specified directory which fit file search criteria. For instance, `C:\logs\*` will scan all files in directory, `C:\logs\*.log` will only scan for files with extension `.log`
* Parses files by reading line by line and testing against subscription's pattern. If line matches the pattern, parser emits corresponding message to Service Bus.
* Listens for system events to be notified for changes according monitoring files. If file modified, renamed or created, the parser scans it again.

### Log Generator Service
Implemented as a Console Application for testing purposes. Generates log files by reading user input.

### Service Bus
Service Bus is used as a robust messaging service for loosely-coupled message-driven components of the system.

There are three topics and three message types in the system:
* Notifications topic for queuing pattern-match notifications for sending. `Monitor Service` emits these messages and `Notification Service` handles them.
* Errors topic for emitting messages about errors occured in the system. `Error-Handling Service` then logs them to Windows Event Log.
* Subscriptions topic for sending messages about subscription updates. `Subscriptions API` track changes and emits messages. `Monitor Services` handles these messages only if machine names of the subscription and client machine match.

## Requirements

OS:
* Windows 8.1
* .Net Framework 4.5

Software:
* Microsoft Visual Studio 2013
* Microsoft SQL Server 2014 Express LocalDB (should be bundled with Visual Studio)
* [Microsoft Service Bus for Windows Server v1.1](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx). Installation instructions also provided [here](#installing-service-bus).

## Technologies
The project powered by:
* .Net Framework 4.5
* ASP.NET MVC 5 and WebApi frameworks
* MSSQL as data storage and Entity Framework as ORM
* Microsoft Unity for IoC
* HTML5/CSS3/Bootstrap and JavaScript/jQuery/Angular.js for Single-Page-Application (SPA)
* Microsoft Service Bus for Windows Server as communication channel and queue

## How to Build
* Open `AwesomeLogger/AwesomeLogger.sln`
* Resotre NuGet packages by clicking right button on solution: `Manage NuGet Packages for Solution...`, then click `Restore` button.
* Right click on Solution: `Rebuild Solution`

## How to Install

### Installing Service Bus
Take a look at the instructions provided by [Microsoft](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx). You can install single Service Bus and then add instances to the farm, so the channel between your sevices will be robust and will not have SPOF (single point of failure).

* Install using Web Platform Installer (search by keywords `Windows Azure Service Bus 1.1.`)
* Follow the instructions and provide required usernames and passwords.
* Open Service Bus PowerShell utility and type: `Get-SBNamespace`. You will see current status of the service bus.
* Create namespace (if not created) and add users and groups to access service bus: `New-SBNamespace -name DemoSB -ManageUsers yourDomain\YourAccount`
* Get connection string to use in services: `Get-SBClientconfiguration –namespace DemoSB`

### Installing Error-Handling service
This service should be installed on server machine. Service can be scaled horizontally by installing on additional machines, since all services connected to the same service bus, they will work together to serve incoming messages.

Specify settings in `App.config`:
* `Microsoft.ServiceBus.ConnectionString` - service bus [connection string](#installing-service-bus).

Install as a Console Application
* Copy all files from `AwesomeLogger\AwesomeLogger.ErrorHandlingService\bin\Release` to server machine.
* Run `AwesomeLogger.ErrorHandlingService.exe` with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.

Install as a Windows Service
* Open `Command Prompt` under Administator account.
* Go to `AwesomeLogger\AwesomeLogger.ErrorHandlingService\bin\Release\Install` directory
* Type `install <domain_name>\<user_name> <password>`, by providing user account credentions with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.
* Windows service `AwesomeLogger Error-Handling Service` should have `Running` status.

### Installing Notification service
This service should be installed on server machine. Service can bee scaled horizontally by installing on additional machines, since all services connected to the same service bus, they will work together to serve incoming messages.

Specify settings in `App.config`:
* `Microsoft.ServiceBus.ConnectionString` - service bus [connection string](#installing-service-bus).
* `SendgridUsername` - [SendGrid](https://sendgrid.com/) account username if you want to send email notifications.
* `SendgridPassword` - SendGrid account password.
* `AuditUri` - address of the [Audit API](#audit-api) service.

Install as a Console Application
* Copy all files from `AwesomeLogger\AwesomeLogger.NotificationService\bin\Release` to server machine.
* Run `AwesomeLogger.NotificationService.exe` with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.

Install as a Windows Service
* Open `Command Prompt` under Administator account.
* Go to `AwesomeLogger\AwesomeLogger.NotificationService\bin\Release\Install` directory
* Type `install <domain_name>\<user_name> <password>`, by providing user account credentions with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.
* Windows service `AwesomeLogger Notification Service` should have `Running` status.

### Installing Monitor service
This service should be installed on each client machine running Windows.

Specify settings in `App.config`:
* `Microsoft.ServiceBus.ConnectionString` - service bus [connection string](#installing-service-bus).
* `SubscriptionsUri` - address of the [Subscriptions API](#subscriptions-api) service.

Install as a Console Application
* Copy all files from `AwesomeLogger\AwesomeLogger.Monitor\bin\Release` to server machine.
* Run `AwesomeLogger.Monitor.exe` with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.

Install as a Windows Service
* Open `Command Prompt` under Administator account.
* Go to `AwesomeLogger\AwesomeLogger.Monitor\bin\Release\Install` directory
* Type `install <domain_name>\<user_name> <password>`, by providing user account credentions with [sufficient permissions](#installing-service-bus) to connect to service bus. For testing purpose you can run service under **current user** account.
* Windows service `AwesomeLogger Monitor Service` should have `Running` status.

## Further Improvements
General thoughts about how to impove the system.

Monitoring service:
* Should persist it's state (byte position in a log file) on the client machine to prevent full re-scan after new configuration received or restart.

## License
The system is distributed under [MIT](LICENSE) license.

## Contacts
For all questions you can reach me via alexey.ernest@gmail.com
