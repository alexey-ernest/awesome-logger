using System.ComponentModel.DataAnnotations;

namespace AwesomeLogger.Subscriptions.Api.Models
{
    public class SubscriptionCreateUpdateModel
    {
        [Required]
        public string MachineName { get; set; }

        [Required]
        public string LogPath { get; set; }

        [Required]
        public string Pattern { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}