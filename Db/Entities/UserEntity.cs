namespace ServiceBusDriver.Db.Entities
{

    public class UserEntity : BaseEntity
    {
        public string TeamName { get; set; }

        public string AuthUserId { get; set; }

        public string Email { get; set; }

        public string EmailOtp { get; set; }

        public bool EmailVerified { get; set; }
    }
}