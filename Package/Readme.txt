AwesomeLogger Application.
Full documentation on https://github.com/alexey-ernest/awesome-logger

Log monitoring and notification system. The system allows its users to monitor intranet 
application logs and receive notifications if specific events occur.

1. Install and configure prerequisites for development environment

Required Software:
* Windows 8.1
* .Net Framework 4.5
* Microsoft Visual Studio 2013
* Microsoft SQL Server 2014 Express LocalDB (should be bundled with Visual Studio)
* Microsoft Service Bus for Windows Server v1.1.

Service Bus installation (https://msdn.microsoft.com/ru-ru/library/dn282152(v=azure.10).aspx):
* You can install single Service Bus and then add instances to the farm, so the channel 
	between your sevices will be robust and will not have SPOF (single point of failure).
* Install using Web Platform Installer (search by keywords Windows Azure Service Bus 1.1.)
* Follow the instructions and provide required usernames and passwords.
* Open Service Bus PowerShell utility and type: Get-SBNamespace. You will see current status of the service bus.
* Create namespace (if not created) and add users and groups to access service bus: 
	New-SBNamespace -name DemoSB -ManageUsers yourDomain\YourAccount
* Get connection string to use in services: Get-SBClientconfiguration –namespace DemoSB


2. Database initialization
Database should be initialized automatially on first run using Entity Framework Code First approach.


3. Prepare source code

Building:
* Open \Source\AwesomeLogger\AwesomeLogger.sln
* Resotre NuGet packages by clicking right button on solution: 
	Manage NuGet Packages for Solution..., then click Restore button.
* Right click on Solution: Rebuild Solution

Installing as Windows Services:
	Error-Handling Service on server machine
	Notification Service on server machine
	Monitor Service on client machine

* Specify settings in App.config: Microsoft.ServiceBus.ConnectionString
* Open Command Prompt under Administator account.
* Go to Source\AwesomeLogger\AwesomeLogger.<ServiceName>\bin\Release\Install directory
* Type: install <domain_name>\<user_name> <password>
	by providing user account credentions with sufficient permissions to connect 
	to service bus. For testing purpose you can run service under current user account.
* Windows service should have Running status.

Installing APIs for testing:
	SubscriptionsApi
	AuditApi

* Specify settings in Web.config: Microsoft.ServiceBus.ConnectionString
* Open solution in Visual Studio
* Right click on project, then select Debug/Start new instance
* Service is running by IIS Express on http://localhost:<port>/ address.

Installing Website:

* Specify settings in Web.config: AdminUsername, AdminPassword (admin/admin by default)
* Open solution in Visual Studio
* Right click on project, then select Debug/Start new instance
* Service is running by IIS Express on http://localhost:1915/ address.


4. Missing requirements not covered in the requirements

1. It's better to decouple Subscriptions and PatternMatching data flows, since the first chaned rarely, the second is heavy load.
2. To allow monitor services on client machines know about configuration updates, to tell about the errors and to decouple 
components I've decided to use Service Bus messaging service.


5. Feedback

1. Instead of configuring WCF services I used ASP.NET WebApi to build RESTfull APIs no to restrict API consumers.
2. I decided to decouple subscriptions and matching patterns into two differrent micro-services. Each one own it's data.
3. Services Bus allows break system into small independed services. Each service can be deployed, versioned and scaled 
independently.
4. For Auditing pattern-matching services it's better to use NoSQL databases, such as Cassandra, since the data is never changed,
there are many writes and low reads.
