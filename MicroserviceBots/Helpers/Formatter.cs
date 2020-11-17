using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeanBot.Helpers
{
    public static class Formatter
    {
        public static LogMessage GenerateLog(Discord.LogMessage message)
        {
            var logItem = "Time: " + DateTime.Now.ToString();
            logItem += " | " + "Severity: " + message.Severity.ToString();
            logItem += " | " + "Source: " + message.Source;
            logItem += " | " + "Message: " + message.Message;
            if (message.Exception != null)
                logItem += " | " + "Exception: " + message.Exception;

            var logmsg = new LogMessage();
            logmsg.message = logItem;

            switch (message.Severity)
            {
                case Discord.LogSeverity.Critical:
                    logmsg.level = LogLevel.Critical;
                    break;
                case Discord.LogSeverity.Error:
                    logmsg.level = LogLevel.Error;
                    break;
                case Discord.LogSeverity.Warning:
                    logmsg.level = LogLevel.Warning;
                    break;
                case Discord.LogSeverity.Info:
                    logmsg.level = LogLevel.Information;
                    break;
                case Discord.LogSeverity.Debug:
                    logmsg.level = LogLevel.Debug;
                    break;
                case Discord.LogSeverity.Verbose:
                    logmsg.level = LogLevel.Trace;
                    break;
                default:
                    logmsg.level = LogLevel.Trace;
                    break;
            }

            return logmsg;
        }

        public static void GenerateLog(ILogger logger, Discord.LogSeverity severity, string source, string message, Exception exception = null)
        {
            var logItem = "Time: " + DateTime.Now.ToString();
            logItem += " | " + "Severity: " + severity.ToString();
            logItem += " | " + "Source: " + source;
            logItem += " | " + "Message: " + message;
            if (exception != null)
                logItem += " | " + "Exception: " + exception;

            switch (severity)
            {
                case Discord.LogSeverity.Critical:
                    logger.Log(LogLevel.Critical, logItem);
                    break;
                case Discord.LogSeverity.Error:
                    logger.Log(LogLevel.Error, logItem);
                    break;
                case Discord.LogSeverity.Warning:
                    logger.Log(LogLevel.Warning, logItem);
                    break;
                case Discord.LogSeverity.Info:
                    logger.Log(LogLevel.Information, logItem);
                    break;
                case Discord.LogSeverity.Debug:
                    logger.Log(LogLevel.Debug, logItem);
                    break;
                case Discord.LogSeverity.Verbose:
                    logger.Log(LogLevel.Trace, logItem);
                    break;
                default:
                    logger.Log(LogLevel.Trace, logItem);
                    break;
            }
        }
    }

    public class LogMessage
    {
        public LogLevel level;
        public string message;
    }
}
