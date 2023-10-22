using System;
using System.IO;

namespace PetFinderService
{
    public static class Logger
    {
        private static readonly string LogFilePath = "../PetFinderServiceLog.txt";

        private static LogLevel CurrentLogLevel = LogLevel.Info;

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        private static void Log(LogLevel level, string? message)
        {
            if (message == null)
            {
                message = "N/A"; // Default value for null messages.
            }

            if (level < CurrentLogLevel) return;

        // Ensure the directory exists.
        string? directory = Path.GetDirectoryName(LogFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
            using StreamWriter sw = new StreamWriter(LogFilePath, true);
            sw.WriteLine($"{DateTime.Now} [{level}] - {message}");
        }

        public static void Debug(object? message)
        {
            Log(LogLevel.Debug, message?.ToString());
        }

        public static void Info(object? message)
        {
            Log(LogLevel.Info, message?.ToString());
        }

        public static void Warning(object? message)
        {
            Log(LogLevel.Warning, message?.ToString());
        }

        public static void Error(object? message)
        {
            Log(LogLevel.Error, message?.ToString());
        }

        public static void SetLogLevel(LogLevel level)
        {
            CurrentLogLevel = level;
        }
    }
}
