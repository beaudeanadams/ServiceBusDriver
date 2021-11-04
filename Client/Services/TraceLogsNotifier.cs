using System;
using System.Text;
using System.Threading.Tasks;
using ServiceBusDriver.Shared.Features.Trace;

namespace ServiceBusDriver.Client.Services
{

    public interface ITraceLogsNotifier
    {
        Task AddToQueue(TraceModel traceModel);
        Task AddToQueue(TraceTypeEnum val, string message);
        
        Task AddToQueue(string message);
        void AddEventListener(Func<Task> listener);
        string GetTraceLogs();

    }

    public class TraceLogsNotifier : ITraceLogsNotifier
    {
        private static StringBuilder TraceLogs { get; } = new StringBuilder();

        public static event Func<Task> Notify;

        public string GetTraceLogs()
        {
            return TraceLogs.ToString() + "<br/><br/>";
        }

        public async Task AddToQueue(string message)
        {
            await AddToQueue(new TraceModel( message));
        }

        public void AddEventListener(Func<Task> listener)
        {
            Notify += listener;
        }

        public async Task AddToQueue(TraceModel traceModel)
        {
            
            var traceLogData = traceModel.ToString();
            TraceLogs.AppendFormat("{0}<br/>", traceLogData);

            if (Notify != null)
            {
                await Notify?.Invoke();
            }
        }

        public async Task AddToQueue(TraceTypeEnum val, string message)
        {
            await AddToQueue(new TraceModel(val, message));
        }
    }
}
