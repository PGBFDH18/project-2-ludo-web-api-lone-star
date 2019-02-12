using System.ComponentModel.DataAnnotations;

namespace Ludo.WebAPI.Models
{
    public class PlayerReady
    {
        [Required]
        public string PlayerId { get; set; }
        [Required]
        public bool Ready { get; set; }
    }
}
