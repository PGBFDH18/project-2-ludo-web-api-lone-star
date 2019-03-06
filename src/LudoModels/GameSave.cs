using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class GameSave
    {
        [Required]
        public BoardState Board { get; set; }
        [Required]
        public TurnSlotDie Current { get; set; }
        [Required]
        public LobbyInfo Lobby { get; set; }
    }
}
