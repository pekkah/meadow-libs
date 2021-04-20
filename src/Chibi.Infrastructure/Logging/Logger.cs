using System;
using System.Diagnostics;

namespace Chibi.Infrastructure.Logging
{
    public readonly struct Logger
    {
        private readonly string _name;

        private Logger(string name)
        {
            _name = name;
        }

        public void LogInfo(string message)
        {
            var name = _name;
            //Task.Run(() => Trace.WriteLine($"[INFO] {DateTime.Now:T} {name}: {message}"));
            Trace.WriteLine($"[INFO] {DateTime.Now:T} {name}: {message}");
        }

        [Conditional("DEBUG")]
        public void LogDebug(string message)
        {
            var name = _name;
            //Task.Run(() => Trace.WriteLine($"[DEBUG] {DateTime.Now:T} {name}: {message}"));
            Trace.WriteLine($"[DEBUG] {DateTime.Now:T} {name}: {message}");
        }

        public static Logger GetLogger(string name)
        {
            return new Logger(name);
        }

        public void LogError(Exception exception, string message)
        {
            LogError($"{message}{Environment.NewLine}{exception}");
        }

        public void LogError(string message)
        {
            var name = _name;
            //Task.Run(() => Trace.WriteLine($"[ERROR] {DateTime.Now:T} {name}: {message}"));
            Trace.WriteLine($"[ERROR] {DateTime.Now:T} {name}: {message}");
        }
    }
}