using System;
using System.ComponentModel.DataAnnotations;

namespace AwesomeLogger.Subscriptions.Api.DAL
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [MaxLength(200)]
        [Required]
        public string MachineName { get; set; }

        [Required]
        public string LogPath { get; set; }

        [Required]
        public string Pattern { get; set; }

        [Required]
        public string Email { get; set; }
    }
}