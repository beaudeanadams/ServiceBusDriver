namespace ServiceBusDriver.Client.Constants
{
    public class ApiConstants
    {
        public static class LocalStorageConstants
        {
            public const string JwtTokenKey = "jwt_token";
        }

        public static class NavigationConstants
        {
            public const string LoginPage = "/login";
            public const string RegisterPage = "/register";
            public const string Dashboard = "/";

        }

        public static class PathConstants
        {
            public const string LoginUser = "api/user/login";
            public const string RegisterUser = "api/user";
            public const string VerifyUser = "api/user/verify";

            public const string ProcessConnectionString = "api/connection/process";
            public const string TestConnection = "api/connection/test";
            public const string AddInstance = "api/instance";
            public const string ListInstances = "api/instance/list";
            public const string GetTopicsInInstance = "api/instance/{0}/topics";
            public const string GetQueuesInInstance = "api/instance/{0}/queues";
            
            public const string GetTopic = "api/topic/get";
            public const string GetQueue = "api/queue/get";
            public const string GetSubscriptionsInTopic = "api/topic/subscriptions";
            
            public const string GetSubscription = "api/subscription/get";
            public const string GetActiveMessages = "api/message/active";
            public const string GetDeadLetterMessages = "api/message/deadletter";
            public const string GetLastNMessages = "api/message/last/{0}";
            public const string SearchMessages = "api/message/search";
        }


        public static class QueryConstants
        {
            public const string InstanceId = "instanceId";
            public const string TopicName = "topicName";
            public const string QueueName = "queueName";
            public const string SubscriptionName = "subscriptionName";
            public const string MaxMessages = "maxMessages";
            public const string DeadLetterQueue = "deadLetterQueue";
        }

        public static class MessagesConstants
        {
            public const int PreFetchCount = 250;
            public const int MaxMessages = 250;
            public const int DefaultMessageLimit = 10;
        }

        public static class UiErrorConstants
        {
            public const string GenericError = "Unknown Error Occured. Please refresh and try again";
            public const string InstanceAndTopicNotFound = "Instance and Topic should be selected";
            public const string InstanceAndQueueNotFound = "Instance and Queue should be selected";
        }

    }
}
