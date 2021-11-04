

using System;

namespace ServiceBusDriver.Shared.Features.Trace
{
    public class TraceModel
    {

        public TraceModel(TraceTypeEnum traceType, string message)
        {
            Message = message;
            CreatedAt = DateTimeOffset.UtcNow;
            Type = traceType;
        }

        public TraceModel(string message)
        {
            Message = message;
            CreatedAt = DateTimeOffset.UtcNow;
            Type = TraceTypeEnum.INFO;
        }

        public string Message { get; set; }
        public TraceTypeEnum Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public override string ToString()
        {
            return $"{CreatedAt} {GetTypeSpan()} {Message}";
        }

        public string GetTypeSpan()
        {
            switch (Type)
            {
                case TraceTypeEnum.DEBUG:
                    return ": <span class='tracelabel-debug' style='background-color: #4d5b9d;padding-left: 3px;padding-right: 3px;border-radius: 3px;color: white;'>DEBUG</span>";

                case TraceTypeEnum.INFO:
                    return ": <span class='tracelabel-info' style='background-color: #58e858;padding-left: 3px;padding-right: 3px;border-radius: 3px;'>INFO</span> -";

                case TraceTypeEnum.WARN:
                    return ": <span class='tracelabel-info' style='background-color: #58e858;padding-left: 3px;padding-right: 3px;border-radius: 3px;'>INFO</span> -";

                case TraceTypeEnum.ERROR:
                    return ": <span class='tracelabel-error' style='background-color: #e85858a3;padding-left: 3px;padding-right: 3px;border-radius: 3px;'>ERROR</span> -";

                case TraceTypeEnum.FATAL:
                    return ": <span class='tracelabel-fatal' style='background-color: #9d4d4d;padding-left: 3px;padding-right: 3px;border-radius: 3px;color: white;'>FATAL</span> -";
                
                default:
                    return ": <span class='tracelabel-debug' style='background-color: #4d5b9d;padding-left: 3px;padding-right: 3px;border-radius: 3px;color: white;'>DEBUG</span>";

            }
        }
    }

    public enum TraceTypeEnum
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }


}