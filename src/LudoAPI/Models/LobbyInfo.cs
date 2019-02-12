using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Ludo.WebAPI.Models
{
    public class LobbyInfo
    {
        [Required]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public GameState State { get; set; }
        [Required]
        public (string Id, int index)[] Players { get; set; }
    }
}
