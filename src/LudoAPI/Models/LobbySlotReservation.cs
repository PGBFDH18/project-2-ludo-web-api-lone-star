using System.ComponentModel.DataAnnotations;

namespace Ludo.WebAPI.Models
{
    public class LobbySlotReservation
    {
        [Required] // TODO: update specification to reflect that this is required.
        public string PlayerId { get; set; }
        public bool Strict { get; set; }
    }
}
