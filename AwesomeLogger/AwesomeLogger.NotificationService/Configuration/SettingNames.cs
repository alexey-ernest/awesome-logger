namespace AwesomeLogger.NotificationService.Configuration
{
    public static class SettingNames
    {
        public static readonly string ServiceBusNotifyTopic = "ServiceBusNotifyTopic";
        public static readonly string ServiceBusNotifyChannel = "ServiceBusNotifyChannel";
        public static readonly string ServiceBusConnectionString = "Microsoft.ServiceBus.ConnectionString";
        public static readonly string SendgridUsername = "SendgridUsername";
        public static readonly string SendgridPassword = "SendgridPassword";
        public static readonly string NotificationAddress = "NotificationAddress";
        public static readonly string AuditUri = "AuditUri";
        public static readonly string AuditAccessToken = "AuditAccessToken";
    }
}