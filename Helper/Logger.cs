using System.Text;

namespace ScheduledCleanup.Helper
{
    public class Logger
    {
        private static readonly object lockObj = new object();
        private static string logDirectory = "logs"; // 日志目录
        private static string logFilePrefix = "log"; // 日志文件前缀

        static Logger()
        {
            // 确保日志目录存在
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        /// <summary>
        /// 获取当前日期对应的日志文件路径
        /// </summary>
        private static string GetCurrentLogFilePath()
        {
            string dateString = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(logDirectory, $"{logFilePrefix}_{dateString}.txt");
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="logLevel">日志级别</param>
        public static void WriteLog(string message, LogLevel logLevel = LogLevel.INFO)
        {
            lock (lockObj) // 确保线程安全
            {
                try
                {
                    string logFilePath = GetCurrentLogFilePath();
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {message}{Environment.NewLine}";

                    // 写入文件
                    File.AppendAllText(logFilePath, logEntry, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    // 如果日志写入失败，可以在这里处理，比如输出到控制台
                    Console.WriteLine($"无法写入日志: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 异步写入日志
        /// </summary>
        public static async Task WriteLogAsync(string message, LogLevel logLevel = LogLevel.INFO)
        {
            await Task.Run(() => WriteLog(message, logLevel));
        }

        /// <summary>
        /// 设置日志目录
        /// </summary>
        public static void SetLogDirectory(string directory)
        {
            lock (lockObj)
            {
                logDirectory = directory;
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
        }

        /// <summary>
        /// 设置日志文件前缀
        /// </summary>
        public static void SetLogFilePrefix(string prefix)
        {
            lock (lockObj)
            {
                logFilePrefix = prefix;
            }
        }
    }

    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        CRITICAL
    }
}
