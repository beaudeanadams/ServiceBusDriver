using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceBusDriver.Core.Models.Errors
{
    public class ErrorMessageModel
    {
        [Required] 
        public string Code { get; set; }

        [Required]
        public DateTimeOffset Timestamp => DateTime.Now;

        public string UserMessageText { get; set; }

        public string SupportReferenceId { get; set; }

        [JsonIgnore] private Exception Exception { get; set; }

        public Exception GetException()
        {
            return Exception;
        }

        public void SetException(Exception e)
        {
            Exception = e;
        }
    }
}