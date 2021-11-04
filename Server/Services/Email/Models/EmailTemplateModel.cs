using System.Collections.Generic;

namespace ServiceBusDriver.Server.Services.Email.Models
{
    public class EmailTemplateModel
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public List<string> Placeholders { get; set; }
    }
}
