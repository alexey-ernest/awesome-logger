using System;
using System.Collections.Generic;
using System.IO;
using AwesomeLogger.Monitor.Events;

namespace AwesomeLogger.Monitor
{
    internal class LogMonitor : ILogMonitor
    {
        private readonly IErrorEventEmitter _errorEventEmitter;
        private readonly string _filePath;
        private readonly string _machineName;
        private readonly IMatchEventEmitter _matchEventEmitter;
        private readonly string _pattern;
        private FileSystemWatcher _watcher;

        public LogMonitor(string machineName, string filePath, string pattern, IErrorEventEmitter errorEventEmitter,
            IMatchEventEmitter matchEventEmitter)
        {
            _machineName = machineName;
            _filePath = filePath;
            _pattern = pattern;
            _errorEventEmitter = errorEventEmitter;
            _matchEventEmitter = matchEventEmitter;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        public void Start()
        {
            try
            {
                var path = Path.GetDirectoryName(_filePath);
                var file = Path.GetFileName(_filePath);

                _watcher = new FileSystemWatcher
                {
                    Path = path,
                    NotifyFilter =
                        NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                        NotifyFilters.DirectoryName,
                    Filter = file
                };

                // Add event handlers
                _watcher.Changed += OnChanged;
                _watcher.Created += OnChanged;
                _watcher.Deleted += OnChanged;
                _watcher.Renamed += OnRenamed;

                // Begin watching
                _watcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Monitoring error for log '{0}' with pattern '{1}': {2}",
                    _filePath, _pattern, e));
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                var parser = new LogParser(_machineName, e.FullPath, _pattern, _matchEventEmitter);
                parser.ParseAsync().Wait();
            }
            catch (Exception ex)
            {
                _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _machineName},
                    {"Error", string.Format("Failed to parse log '{0}': {1}", _filePath, ex)}
                }).Wait();
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            try
            {
                var parser = new LogParser(_machineName, e.FullPath, _pattern, _matchEventEmitter);
                parser.ParseAsync().Wait();
            }
            catch (Exception ex)
            {
                _errorEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _machineName},
                    {"Error", string.Format("Failed to parse log '{0}': {1}", _filePath, ex)}
                }).Wait();
            }
        }
    }
}