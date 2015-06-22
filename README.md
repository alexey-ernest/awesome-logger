# Awesome Logger
Log monitoring and notification system. The system allows its users to monitor intranet application logs and receive notifications if specific events occur.

* [Architecture](#architecture)
* [Components](#components)
* [Requirements](#requirements)
* [How to Build](#how-to-build)
* [How to Install](#how-to-install)
* [Further Improvements](#further-improvements)
* [License](#license)
* [Contacts](#contacts)

## Architecture
The system is built using Event-Driven-Architecture (EDA) and Service-Oriented-Architecture (SOA) principles. The system is breaked up into small micro-services, independent separated pieces with single responsibility. Each service owns its data (diffenerent data storages can be used for each micro-service), can be versioned, updated and deployed separateley and can be scaled up horizontally independently if needed. 

The system is distributed and designed without Single-Point-Of-Failure (SPOF). Each component can be scaled and load balanced. For Audit DB NoSQL database is recommended because of high load and log-like structure (heavy write, no changes, low read). Cassandra can be perfectly meet this data usage pattern.

![](Assets/AwesomeLogger-Architecture.png?raw=true)

Data flow:
1.  When a `Monitor Service` started its making a request to `Subscriptions API` to get subscription parameters for client machine.

For security reasons `Subscriptions API` should use SSL to encrypt messages. Also, communication between client and API is protected with access token, which is specified in configuration files on both sides: for `Monitor Service` its an *AccessToken* in App.config, for `Subscriptions API` sevice its an *ExternalAccessToken* in Web.config.
    
2. `Subscription API` executes query agains `Subscriptions DB` to find any subscription parmaeters for client machine. If there are any API sends them to client machine and `Monitor Service` starts parsing logs.
3. If `Monitor Service` finds any match it emits special message to `Service Bus`. Connection between `Service Bus` and publishers/subscribers is encrypted and configured while installation process.
4. 

## Components
The system consists of several services and website. All components are loosely coupled and can be deployed and upgraded independently. 

### Web UI

### Subscriptions API

### Service Bus

### Monitoring Service

### Error-Handling Service

### Audit API

### Notification Service

### Log Generator Service

## Requirements

OS:
* Windows 8.1
* .Net Framework 4.5

Software:
* Microsoft Visual Studio 2013
* Microsoft SQL Server 2014 Express LocalDB (should be bundled with Visual Studio)
* [Microsoft Service Bus for Windows Server v1.1](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx). Installation instructions also provided [here](#installing-service-bus).

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
This service should be installed on server machine. Service can bee scaled horizontally by installing on additional machines, since all services connected to the same service bus, they will work together to serve incoming messages.

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
