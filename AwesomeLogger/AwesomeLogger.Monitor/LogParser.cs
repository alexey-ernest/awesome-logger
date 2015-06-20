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
        private readonly string _filePath;
        private readonly string _machineName;
        private readonly string _pattern;
        private readonly string _emailToNotify;
        private readonly IMatchEventEmitter _matchEventEmitter;

        public LogParser(string machineName, string filePath, string pattern, string emailToNotify, IMatchEventEmitter matchEventEmitter)
        {
            _filePath = filePath;
            _pattern = pattern;
            _matchEventEmitter = matchEventEmitter;
            _emailToNotify = emailToNotify;
            _machineName = machineName;
        }

        public async Task ParseAsync()
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
                        lineNumber++;
                        if (!regex.IsMatch(line))
                        {
                            continue;
                        }

                        // emitting match event in parallel 
                        var match = line;
                        NotifyInBackground(match, lineNumber);
                    }
                }
            }
        }

        private void NotifyInBackground(string match, int lineNumber)
        {
            Task.Run(async () =>
                await _matchEventEmitter.EmitAsync(new Dictionary<string, string>
                {
                    {"MachineName", _machineName},
                    {"Path", _filePath},
                    {"Match", match},
                    {"Line", lineNumber.ToString()},
                    {"Email", _emailToNotify}
                }));
        }
    }
}