using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ludo.API.Models
{
    public class LobbyInfo
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public GamePhase State { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; }
        public PlayerReady[] Slots { get; set; }
        public IReadOnlyList<string> Others { get; set; }
        public LobbyReservation[] Reservations { get; set; }
    }
}
