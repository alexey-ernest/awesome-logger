using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string _emailToNotify;

        public LogMonitor(string machineName, string filePath, string pattern, string emailToNotify, IErrorEventEmitter errorEventEmitter,
            IMatchEventEmitter matchEventEmitter)
        {
            _machineName = machineName;
            _filePath = filePath;
            _pattern = pattern;
            _errorEventEmitter = errorEventEmitter;
            _matchEventEmitter = matchEventEmitter;
            _emailToNotify = emailToNotify;
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }

        public async Task StartAsync()
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

                // Watching for changes
                _watcher.Changed += OnChanged;
                _watcher.Created += OnChanged;
                _watcher.Deleted += OnChanged;
                _watcher.Renamed += OnRenamed;

                _watcher.EnableRaisingEvents = true;

                // Scan logs
                await ScanAsync(_filePath);
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Monitoring error for log '{0}' with pattern '{1}': {2}",
                    _filePath, _pattern, e));
            }
        }

        private async Task ScanAsync(string path)
        {
            var baseDir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(baseDir))
            {
                return;
            }

            var filePattern = Path.GetFileName(path);
            if (!Directory.Exists(baseDir) && !File.Exists(baseDir))
            {
                // path does not exist
                return;
            }

            var attr = File.GetAttributes(baseDir);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                // directory, read files
                var di = new DirectoryInfo(baseDir);
                var files = di.GetFiles(filePattern, SearchOption.TopDirectoryOnly);

                // scanning files
                var scanningTasks = files.Select(fileInfo => ParseAsync(fileInfo.FullName)).ToList();
                await Task.WhenAll(scanningTasks);
            }
            else
            {
                // file
                await ParseAsync(baseDir);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            ParseAsync(e.FullPath).Wait();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            ParseAsync(e.FullPath).Wait();
        }

        private async Task ParseAsync(string filePath)
        {
            try
            {
                var parser = new LogParser(_machineName, filePath, _pattern, _emailToNotify, _matchEventEmitter);
                await parser.ParseAsync();
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