namespace ServiceBusDriver.Shared.Features.User
{
    public class LoginResponseDto
    {
        public string TeamName { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string AuthId { get; set; }
        public string Token { get; set; }
    }
}