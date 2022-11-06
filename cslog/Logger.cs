using System;
using System.IO;
using System.Threading.Tasks;

namespace cslog
{
    public enum LogLevel
    {
        None = 0,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class Logger
    {
        public delegate void LoggingMethod(string message);

        private LoggingMethod LogMethod;
        private bool createLogFile;

        public string LogPath;
        public string Name;
        public LogPattern Pattern;

        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message) => Log(LogLevel.Error, message);
        public void LogFatal(string message) => Log(LogLevel.Fatal, message);
        public void LogDebug(string message, LogPattern usePattern) => Log(LogLevel.Debug, message, usePattern);
        public void LogInfo(string message, LogPattern usePattern) => Log(LogLevel.Info, message, usePattern);
        public void LogWarning(string message, LogPattern usePattern) => Log(LogLevel.Warning, message, usePattern);
        public void LogError(string message, LogPattern usePattern) => Log(LogLevel.Error, message, usePattern);
        public void LogFatal(string message, LogPattern usePattern) => Log(LogLevel.Fatal, message, usePattern);

        public void Log(LogLevel logLevel, string message)
        {
            string s = Pattern.Parse(message, logLevel, this);

            if (createLogFile)
            {
                string code = WriteToFile(s).Result;
            }

            LogMethod(s);
        }

        public void Log(LogLevel logLevel, string message, LogPattern usePattern)
        {
            string s = usePattern.Parse(message, logLevel, this);

            if (createLogFile)
            {
                string code = WriteToFile(s).Result;
            }

            LogMethod(s);
        }

        public static Logger Create(string name, LogPattern pattern)
        {
            Logger logger = new Logger();

            logger.Name = name;
            logger.Pattern = pattern;

            logger.createLogFile = false;

            return logger;
        }

        public static Logger Create(string name, LogPattern pattern, string logPath)
        {
            Logger logger = new Logger();

            logger.Name = name;
            logger.Pattern = pattern;
            logger.LogPath = logPath;

            logger.createLogFile = true;

            return logger;
        }

        public void SetLogMethod(LoggingMethod method)
        {
            LogMethod = method;
        }

        private async Task<string> WriteToFile(string s)
        {
            await File.AppendAllTextAsync(LogPath, s);
            return s;
        }
    }
}
