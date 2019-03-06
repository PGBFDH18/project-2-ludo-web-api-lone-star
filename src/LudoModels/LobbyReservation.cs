using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class LobbyReservation
    {
        [Required]
        public string Player { get; set; }
        [Required]
        public int Slot { get; set; }
        public bool Strict { get; set; }
    }
}
