using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class LobbySave
    {
        [Required]
        public PlayerSlot[] Players { get; set; }
    }
}
