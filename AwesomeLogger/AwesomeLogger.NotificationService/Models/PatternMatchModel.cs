using System;

namespace AwesomeLogger.NotificationService.Models
{
    public class PatternMatchModel
    {
        public DateTime Created { get; set; }

        public string MachineName { get; set; }

        public string SearchPath { get; set; }

        public string LogPath { get; set; }

        public string Pattern { get; set; }

        public int Line { get; set; }

        public string Email { get; set; }

        public string Match { get; set; }

    }
}