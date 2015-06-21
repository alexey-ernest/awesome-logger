using System;
using System.ComponentModel.DataAnnotations;

namespace AwesomeLogger.Audit.Api.DAL
{
    public class PatternMatch
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [MaxLength(200)]
        [Required]
        public string MachineName { get; set; }

        [MaxLength(255)]
        [Required]
        public string SearchPath { get; set; }

        [MaxLength(255)]
        [Required]
        public string LogPath { get; set; }

        [MaxLength(200)]
        [Required]
        public string Pattern { get; set; }

        [Required]
        public int Line { get; set; }

        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Match { get; set; }

    }
}