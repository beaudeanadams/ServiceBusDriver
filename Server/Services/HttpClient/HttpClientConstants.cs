namespace ServiceBusDriver.Server.Services.HttpClient
{
    public class HttpClientConstants
    {
        public class Clients
        {
            public static string SendInBlueClient = "SendInBlue";
            public static string DisposableEmailClient = "DisposableEmailClient";
        }

        public class DisposableEmail
        {
            public static string BaseUrl = "https://open.kickbox.com";
        }
        
        public class SendInBlue
        {
            public static string BaseUrl = "https://api.sendinblue.com/v3";
        }

        public class Paths
        {

            public static string ForgotPasswordEmail = "/auth/invite-password?key={key}";
            public static string VerifyEmail = "/auth/invite-password?key={key}";
            public static string InviteEmail = "/auth/invite-password?key={key}";

            public static string SendTransactionEmailPath = "/smtp/email";
            public static string SendTransactionSmsPath = "/transactionalSMS/sms";
            public static string GetSendInBlueStats = "/smtp/statistics/aggregatedReport";
            public static string GetSendInBlueHistory = "/smtp/statistics/events";

        }
    }
}

