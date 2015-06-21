using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwesomeLogger.Monitor.Events;

namespace AwesomeLogger.Monitor
{
    internal class LogParser : ILogParser
    {
        private readonly string _emailToNotify;
        private readonly string _filePath;
        private readonly string _machineName;
        private readonly IMatchEventEmitter _matchEventEmitter;
        private readonly string _pattern;
        private readonly string _searchPath;
        private bool _isDisposed;

        public LogParser(string machineName, string searchPath, string filePath, string pattern, string emailToNotify,
            IMatchEventEmitter matchEventEmitter)
        {
            _filePath = filePath;
            _searchPath = searchPath;
            _pattern = pattern;
            _matchEventEmitter = matchEventEmitter;
            _emailToNotify = emailToNotify;
            _machineName = machineName;
        }

        public async Task ParseAsync()
        {
            var attemptsCount = 5;
            while (true)
            {
                try
                {
                    // read file line by line
                    var regex = new Regex(_pattern);
                    using (
                        var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1048576,
                            FileOptions.Asynchronous))
                    {
                        using (var file = new StreamReader(fileStream, Encoding.UTF8, true, 1024))
                        {
                            string line;
                            var lineNumber = 0;
                            while ((line = await file.ReadLineAsync()) != null)
                            {
                                if (_isDisposed)
                                {
                                    break;
                                }

                                lineNumber++;
                                if (!regex.IsMatch(line))
                                {
                                    continue;
                                }

                                // emitting match event in parallel 
                                var match = line;
                                NotifyInBackground(_pattern, match, lineNumber);
                            }
                        }
                    }

                    break;
                }
                catch (Exception)
                {
                    if (attemptsCount == 0)
                    {
                        throw;
                    }

                    // trying again
                    Task.Delay(30000).Wait();
                    attemptsCount--;
                }
            }
            
        }

        public void Dispose()
        {
            _isDisposed = true;
        }

        private void NotifyInBackground(string pattern, string match, int lineNumber)
        {
            Task.Run(async () =>
                await _matchEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _machineName},
                    {"SearchPath", _searchPath},
                    {"Path", _filePath},
                    {"Pattern", pattern},
                    {"Match", match},
                    {"Line", lineNumber.ToString()},
                    {"Email", _emailToNotify}
                }));
        }
    }
}