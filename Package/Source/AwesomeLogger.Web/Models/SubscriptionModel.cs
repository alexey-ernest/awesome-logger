using System;
using System.ComponentModel.DataAnnotations;

namespace AwesomeLogger.Web.Models
{
    public class SubscriptionModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [MaxLength(200)]
        [Required]
        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }

        [Required]
        [Display(Name = "Log Path Pattern")]
        public string LogPath { get; set; }

        [Required]
        [Display(Name = "Line Regex Pattern")]
        public string Pattern { get; set; }

        [Required]
        [Display(Name = "Notification Email")]
        public string Email { get; set; }
    }
}