using System.Collections.Generic;

namespace ServiceBusDriver.Server.Services.Email.Models
{
    public class EmailRequestModel
    {
        public Sender Sender { get; set; }
        public List<Destination> To { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }

    public class Sender
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
