using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ludo.API.Models
{
    public class CreateLobby
    {
        [Required]
        public string UserId { get; set; }
        public int? Slots { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; } = LobbyAccess.@public;
        public LobbyReservation[] Reservations { get; set; }
    }
}
