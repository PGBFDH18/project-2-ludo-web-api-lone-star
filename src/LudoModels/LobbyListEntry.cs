using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ludo.API.Models
{
    public class LobbyListEntry
    {
        public string GameId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public LobbyAccess Access { get; set; }
        public string[] Slots { get; set; }
        public IReadOnlyList<string> Others { get; set; }
    }
}
