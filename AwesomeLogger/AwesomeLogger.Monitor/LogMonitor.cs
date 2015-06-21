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
        private readonly string _emailToNotify;
        private readonly IErrorEventEmitter _errorEventEmitter;
        private readonly string _filePath;
        private readonly string _machineName;
        private readonly IMatchEventEmitter _matchEventEmitter;
        private readonly string _pattern;
        private FileSystemWatcher _watcher;
        private List<ILogParser> _parsers;

        public LogMonitor(string machineName, string filePath, string pattern, string emailToNotify,
            IErrorEventEmitter errorEventEmitter,
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

            if (_parsers != null)
            {
                foreach (var parser in _parsers)
                {
                    parser.Dispose();
                }
            }
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

                // Watching for changes
                _watcher.Changed += OnChanged;
                _watcher.Created += OnChanged;
                _watcher.Deleted += OnChanged;
                _watcher.Renamed += OnRenamed;

                _watcher.EnableRaisingEvents = true;

                // Scan logs
                _parsers = Scan(_filePath);
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Monitoring error for log '{0}' with pattern '{1}': {2}",
                    _filePath, _pattern, e));
            }
        }

        private List<ILogParser> Scan(string path)
        {
            var baseDir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(baseDir))
            {
                return new List<ILogParser>();
            }

            var filePattern = Path.GetFileName(path);
            if (!Directory.Exists(baseDir) && !File.Exists(baseDir))
            {
                // path does not exist
                return new List<ILogParser>();
            }

            var attr = File.GetAttributes(baseDir);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                // directory, read files
                var di = new DirectoryInfo(baseDir);
                var files = di.GetFiles(filePattern, SearchOption.TopDirectoryOnly);

                // scanning files
                var parsers = files.Select(fileInfo => Parse(fileInfo.FullName)).ToList();
                return parsers;
            }

            // file
            return new List<ILogParser> {Parse(baseDir)};
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Parse(e.FullPath);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Parse(e.FullPath);
        }

        private ILogParser Parse(string filePath)
        {
            var parser = new LogParser(_machineName, filePath, _pattern, _emailToNotify, _matchEventEmitter);

            // parsing in parallel
            Task.Run(async () =>
            {
                try
                {
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
            });

            return parser;
        }
    }
}