using NLog;

namespace Common.Implementation
{
    public class ActivityLogger
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static ActivityLogger? mInstance;
        public static ActivityLogger Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new ActivityLogger();
                }
                return mInstance;
            }
        }

        public ActivityLogger()
        {
            string logLevel = string.Empty;
            IList<NLog.Config.LoggingRule> lstLoggingRules = LogManager.Configuration.LoggingRules;
            if (lstLoggingRules != null)
            {
                NLog.Config.LoggingRule loggingRule = lstLoggingRules[0];
                CurrentLogLevel = loggingRule.Levels[0];
            }
        }

        public LogLevel CurrentLogLevel { get; set; }

        public void SystemLog(LogLevel level, string message, string action,string correlationId, string userId,string machineName, string eventOrigin, string description, int result, Exception ex = null)
        {
            ActivityLogEvent logEvent = new ActivityLogEvent();

            logger.Log(logEvent.GetLogEvent(level,"Service Name",message,action, correlationId, "",DateTime.Now,"",DateTime.Now,machineName,result,eventOrigin,description,Interface.ActionType.Undefined,ex));
        }
        public static void LogInfo(string message)
        {
            logger.Info(message);
        }
        public static void LogError(string message, Exception ex)
        {
            logger.Error(ex, message);
        }
        public static void LogDebug(string message)
        {
            logger.Debug(message);
        }
    }
}
