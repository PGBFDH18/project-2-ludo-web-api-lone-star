using System.ComponentModel.DataAnnotations;

namespace Ludo.WebAPI.Models
{
    public class LobbySave
    {
        [Required]
        public PlayerSlot[] Players { get; set; }
    }
}
