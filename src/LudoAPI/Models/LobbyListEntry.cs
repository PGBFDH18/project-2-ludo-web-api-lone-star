using Newtonsoft.Json;

namespace Ludo.WebAPI.Models
{
    public struct LobbyListEntry
    {
        public string GameId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; }
        public LobbySlot[] Slots { get; set; }
    }
}
