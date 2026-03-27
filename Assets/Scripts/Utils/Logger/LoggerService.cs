using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

/// <summary>
/// High-performance, reusable logger service for Unity.
/// Optimized for minimal allocations using object pooling and efficient string formatting.
/// 
/// This logger is fully compatible with Unity's built-in Debug logging system.
/// All logs are output through UnityEngine.Debug, so they appear in Unity's Console window
/// and can be filtered/controlled through Unity's standard logging mechanisms.
/// 
/// No conflicts with Unity's Debug class - uses explicit UnityEngine.Debug internally.
/// </summary>
public static class LoggerService
{
    private static readonly StringBuilder _stringBuilder = new StringBuilder(256);
    private static readonly object _lockObject = new object();

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }

    /// <summary>
    /// Logs a debug message with automatic file and line number detection.
    /// Compatible with Unity's Debug.Log - outputs through Unity's console system.
    /// </summary>
    public static void LogDebug(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
#if UNITY_EDITOR
        Debug.Log(message);
#else

        Log(LogLevel.DEBUG, message, filePath, lineNumber, memberName);
#endif
    }

    /// <summary>
    /// Logs an info message with automatic file and line number detection.
    /// </summary>
    public static void Info(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
        Log(LogLevel.INFO, message, filePath, lineNumber, memberName);
    }

    /// <summary>
    /// Logs a warning message with automatic file and line number detection.
    /// </summary>
    public static void Warning(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#else
        Log(LogLevel.WARNING, message, filePath, lineNumber, memberName);
#endif
    }

    /// <summary>
    /// Logs an error message with automatic file and line number detection.
    /// </summary>
    public static void Error(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#else
        Log(LogLevel.ERROR, message, filePath, lineNumber, memberName);
#endif
    }

    /// <summary>
    /// Logs an exception with automatic file and line number detection.
    /// </summary>
    public static void Exception(
        Exception exception,
        string context = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
        string message = string.IsNullOrEmpty(context)
            ? exception.ToString()
            : $"{context}: {exception}";

#if UNITY_EDITOR
        Debug.LogError(message);
#else
        Log(LogLevel.ERROR, message, filePath, lineNumber, memberName);
#endif
    }

    /// <summary>
    /// Core logging method that formats and outputs the log message.
    /// </summary>
    private static void Log(
        LogLevel level,
        string message,
        string filePath,
        int lineNumber,
        string memberName)
    {
        lock (_lockObject)
        {
            _stringBuilder.Clear();

            // Format: 2026-01-22T08:51:29.200Z - [ERROR] (ApplicationEntryPoint.cs:140) Exception during AcceptPlayerSession: System.Exception: Simulated exception for testing purposes.
            
            // Timestamp in ISO 8601 format with milliseconds
            DateTime now = DateTime.UtcNow;
            _stringBuilder.Append(now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            _stringBuilder.Append(" - [");
            _stringBuilder.Append(level.ToString());
            _stringBuilder.Append("] (");

            // Extract filename from full path
            string fileName = ExtractFileName(filePath);
            _stringBuilder.Append(fileName);
            _stringBuilder.Append(':');
            _stringBuilder.Append(lineNumber);
            _stringBuilder.Append(") ");

            // Add the actual message
            _stringBuilder.Append(message);

            string logMessage = _stringBuilder.ToString();

            // Output to Unity's console with appropriate log type
            switch (level)
            {
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                    Debug.Log(logMessage);
                    break;
                case LogLevel.WARNING:
                    Debug.LogWarning(logMessage);
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(logMessage);
                    break;
            }
        }
    }

    /// <summary>
    /// Extracts the filename from a full file path efficiently.
    /// </summary>
    private static string ExtractFileName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return "Unknown";

        // Find the last directory separator
        int lastSeparator = Math.Max(
            filePath.LastIndexOf('/'),
            filePath.LastIndexOf('\\'));

        return lastSeparator >= 0 && lastSeparator < filePath.Length - 1
            ? filePath.Substring(lastSeparator + 1)
            : filePath;
    }
}
