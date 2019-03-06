using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class LoadLobby
    {
        [Required]
        public GameSave Save { get; set; }
        public LobbyAccess Access { get; set; }
        public bool Strict { get; set; } // refers to reservations
    }
}
