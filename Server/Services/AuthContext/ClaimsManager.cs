using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace ServiceBusDriver.Server.Services.AuthContext
{
    public class ClaimsManager : IClaimsManager
    {
        private JwtPayload _payload;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            var headers = _httpContextAccessor.HttpContext.Request.Headers;

            var authToken = headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");


            var jwtToken = new JwtSecurityToken(authToken);
            _payload = jwtToken.Payload;
            // jwtToken.Payload.GetValueOrDefault("user_id");
        }

        public string GetEmailVerified()
        {
            return _payload.GetValueOrDefault("email_verified").ToString();
        }

        public string GetEmail()
        {
            return _payload.GetValueOrDefault("email").ToString();
        }

        public string GetUserId()
        {
            return _payload.GetValueOrDefault("user_id").ToString();
        }
    }
}