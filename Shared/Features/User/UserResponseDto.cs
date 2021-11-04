namespace ServiceBusDriver.Shared.Features.User
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string AuthUserId { get; set; }
        public string TeamName { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}