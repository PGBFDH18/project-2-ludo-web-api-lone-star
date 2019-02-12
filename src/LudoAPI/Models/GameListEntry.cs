using Newtonsoft.Json;

namespace Ludo.WebAPI.Models
{
    public class GameListEntry
    {
        public string GameId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public GameState State { get; set; }
        public string Winner { get; set; }
        public string[] Players { get; set; }
    }
}
