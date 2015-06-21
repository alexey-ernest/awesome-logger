# Awesome Logger
Log monitoring and notification system. The system allows its users to monitor intranet application logs and receive notifications if specific events occur.

## Requirements

Software:
* Microsoft Visual Studio 2013
* [Microsoft Service Bus for Windows Server v1.1](https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx)
* Microsoft SQL Server 2014 Express LocalDB
* 


## How to Build

## How to Install

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
