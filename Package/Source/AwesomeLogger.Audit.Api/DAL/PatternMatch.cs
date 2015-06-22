using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwesomeLogger.Audit.Api.DAL
{
    public class PatternMatch
    {
        public int Id { get; set; }

        [Index("IX_SearchIndex", 1)]
        [Index("IX_Subscription", 1)]
        [MaxLength(200)]
        [Required]
        public string MachineName { get; set; }

        [Index("IX_SearchIndex", 2)]
        [Index("IX_Subscription", 2)]
        [MaxLength(255)]
        [Required]
        public string SearchPath { get; set; }

        [Index("IX_SearchIndex", 3)]
        [Index("IX_Subscription", 3)]
        [MaxLength(200)]
        [Required]
        public string Pattern { get; set; }

        [Index("IX_SearchIndex", 4)]
        [Index("IX_Subscription", 4)]
        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [Index("IX_SearchIndex", 5)]
        [MaxLength(255)]
        [Required]
        public string LogPath { get; set; }

        [Index("IX_SearchIndex", 6)]
        [Required]
        public int Line { get; set; }

        [Index("IX_SearchIndex", 7)]
        [MaxLength(1024)]
        [Required]
        public string Match { get; set; }

        [Index("IX_Subscription", 5)]
        [Required]
        public DateTime Created { get; set; }
    }
}