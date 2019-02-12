using System.ComponentModel.DataAnnotations;

namespace Ludo.WebAPI.Models
{
    public class GameSave
    {
        [Required]
        public BoardState Board { get; set; }
        [Required]
        public PlayerDie Current { get; set; }
        [Required]
        public LobbyInfo Lobby { get; set; }
    }
}
