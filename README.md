# Awesome Logger
Log monitoring and notification system. The system allows its users to monitor intranet application logs and receive notifications if specific events occur.

## Requirements

OS:
* Windows 8.1
* .Net Framework 4.5

Software:
* Microsoft Visual Studio 2013
* Microsoft SQL Server 2014 Express LocalDB (should be bundled with Visual Studio)
* [Microsoft Service Bus for Windows Server v1.1](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx)

## How to Build
* Open `AwesomeLogger/AwesomeLogger.sln`
* Resotre NuGet packages by clicking right button on solution: `Manage NuGet Packages for Solution...`, then click `Restore` button.
* Right click on Solution: `Rebuild Solution`

## How to Install

# Installing Service Bus
Take a look at the instructions provided by [Microsoft](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx). You can install single Service Bus and then add instances to the farm, so the channel between your sevices will be robust and will not have SPOF (single point of failure).

* Install using Web Platform Installer (search by keywords `Windows Azure Service Bus 1.1.`)
* Follow the instructions and provide required usernames and passwords.
* Open Service Bus PowerShell utility and type: `Get-SBNamespace`. You will see current status of the service bus.
* Create namespace (if not created) and add users and groups to access service bus: `New-SBNamespace -name DemoSB -ManageUsers yourDomain\YourAccount`
* Get connection string to use in services: `Get-SBClientconfiguration –namespace DemoSB`

## Architecture

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

## Further Improvements
General thoughts about how to impove the system.

Monitoring service:
* Should persist it's state (byte position in a log file) on the client machine to prevent full re-scan after new configuration received or restart.

## Licence
The system is distributed under [MIT](LICENSE) license.

## Contacts
For all questions you can reach me via alexey.ernest@gmail.com
