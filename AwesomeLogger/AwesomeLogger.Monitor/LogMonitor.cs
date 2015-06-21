using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AwesomeLogger.Monitor.Events;

namespace AwesomeLogger.Monitor
{
    internal class LogMonitor : ILogMonitor
    {
        private readonly string _emailToNotify;
        private readonly IErrorEventEmitter _errorEventEmitter;
        private readonly ILogParserFactory _logParserFactory;
        private readonly string _machineName;
        private readonly string _pattern;
        private readonly string _searchPath;
        private readonly ConcurrentDictionary<string, ILogParser> _parsers = new ConcurrentDictionary<string, ILogParser>();
        private FileSystemWatcher _watcher;

        public LogMonitor(string machineName, string searchPath, string pattern, string emailToNotify,
            IErrorEventEmitter errorEventEmitter, ILogParserFactory logParserFactory)
        {
            _machineName = machineName;
            _searchPath = searchPath;
            _pattern = pattern;
            _errorEventEmitter = errorEventEmitter;
            _logParserFactory = logParserFactory;
            _emailToNotify = emailToNotify;
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();        
            }

            if (_parsers != null)
            {
                foreach (var parser in _parsers)
                {
                    parser.Value.Dispose();
                }
                _parsers.Clear();
            }
        }

        public void Start()
        {
            try
            {
                var path = Path.GetDirectoryName(_searchPath);
                var file = Path.GetFileName(_searchPath);

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
                _watcher.Renamed += OnChanged;

                _watcher.EnableRaisingEvents = true;

                // Scan logs
                Scan(_searchPath);
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Monitoring error for log '{0}' with pattern '{1}': {2}",
                    _searchPath, _pattern, e));
            }
        }

        private void Scan(string path)
        {
            var baseDir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(baseDir))
            {
                return;
            }

            var filePattern = Path.GetFileName(path);
            var attr = File.GetAttributes(baseDir);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                // directory
                
                // read files
                var di = new DirectoryInfo(baseDir);
                var files = di.GetFiles(filePattern, SearchOption.TopDirectoryOnly);

                // scanning files
                foreach (var file in files)
                {
                    var parser = Parse(file.FullName);
                    _parsers.AddOrUpdate(file.FullName, parser, (s, logParser) =>
                    {
                        logParser.Dispose();
                        return parser;
                    });
                }
            }
            else
            {
                // file
                var fileParser = Parse(baseDir);
                _parsers.AddOrUpdate(baseDir, fileParser, (s, logParser) =>
                {
                    logParser.Dispose();
                    return fileParser;
                });
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var parser = Parse(e.FullPath);
            _parsers.AddOrUpdate(e.FullPath, parser, (s, logParser) =>
            {
                logParser.Dispose();
                return parser;
            });
        }

        private ILogParser Parse(string filePath)
        {
            var parser = _logParserFactory.Create(_machineName, _searchPath, filePath, _pattern, _emailToNotify);

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
                        {"Error", string.Format("Failed to parse log '{0}': {1}", _searchPath, ex)}
                    }).Wait();
                }
            });

            return parser;
        }
    }
}