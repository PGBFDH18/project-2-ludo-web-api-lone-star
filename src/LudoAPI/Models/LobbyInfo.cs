using Newtonsoft.Json;

namespace Ludo.WebAPI.Models
{
    public class LobbyInfo
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public GameState State { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; }
        public PlayerReady[] Slots { get; set; }
        public string[] Others { get; set; }
        public LobbyReservation[] Reservations { get; set; }
    }
}
