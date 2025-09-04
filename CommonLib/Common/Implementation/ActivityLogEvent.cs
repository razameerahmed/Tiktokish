using Common.Interface;
using NLog;

namespace Common.Implementation
{
    class ActivityLogEvent
    {
        public LogEventInfo GetLogEvent(LogLevel level, string loggerName, string message, string Action,string CorrelationId, string CreatedBy, DateTime CreatedOn, string UpdateBy, DateTime UpdatedOn, string MachineName, int Result, string EventOrigin, string Description, ActionType actionType, Exception ex)
        {
            var logEvent = new LogEventInfo(level, loggerName, message);

            //logEvent.Properties["ENTITY"] = Entity;
            logEvent.Properties["ACTION"] = Action;
            //logEvent.Properties["PRIMARY_KEY_VALS"] = PrimaryKeyVals;
            logEvent.Properties["CorrelationId"] = CorrelationId;
            logEvent.Properties["CREATED_ON"] = CreatedOn;
            logEvent.Properties["CREATED_BY"] = CreatedBy;
            logEvent.Properties["UPDATED_ON"] = UpdatedOn;
            logEvent.Properties["UPDATED_BY"] = UpdateBy;
            logEvent.Properties["WINDOWS_CREATED_BY"] = "";
            logEvent.Properties["WINDOWS_UPDATED_BY"] = "";
            logEvent.Properties["MACHINE_NAME"] = MachineName;
            logEvent.Properties["LOG_TYPE_ID"] = ((int)LogType.Activity).ToString();
            logEvent.Properties["RESULT"] = Result;
            logEvent.Properties["EVENT_ORIGIN"] = EventOrigin;
            logEvent.Properties["DESCRIPTION"] = Description;
            logEvent.Properties["ACTION_TYPE"] = (int)actionType;
            logEvent.Properties["LEVEL"] = level.Ordinal;
            logEvent.Properties["LOGGER_NAME"] = loggerName;
            logEvent.Properties["MESSAGE"] = message;
            if (ex != null)
                logEvent.Properties["EXCEPTION"] = ex.ToString();
            else
                logEvent.Properties["EXCEPTION"] = "";
            return logEvent;
        }
    }

}
