using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class PlayerReady
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool Ready { get; set; }
    }
}
