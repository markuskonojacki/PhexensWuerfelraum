using NLog;
using NLog.Config;
using NLog.Targets;

namespace PhexensWuerfelraum.Logic.Ui
{
    public static class Log
    {
        public static Logger Instance { get; private set; }

        static Log()
        {
#if DEBUG
            // Setup the logging view for Sentinel - https://github.com/yarseyah/sentinel
            var sentinalTarget = new NLogViewerTarget()
            {
                Name = "sentinel",
                Address = "udp://127.0.0.1:9999",
                IncludeNLogData = false
            };
            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
            LogManager.Configuration.AddTarget("sentinel", sentinalTarget);
            LogManager.Configuration.LoggingRules.Add(sentinalRule);
#endif

            LogManager.ReconfigExistingLoggers();

            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}