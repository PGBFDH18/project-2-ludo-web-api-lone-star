using Newtonsoft.Json;

namespace Ludo.WebAPI.Models
{
    public class LobbyListEntry
    {
        public string GameId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; }
        public string[] Slots { get; set; }
        public string[] Others { get; set; }
    }
}
