using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Ludo.API.Models
{
    public class GameListEntry : IEnumerable<GameListEntry>
    {
        public string GameId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))] // serialize as string!
        public GamePhase State { get; set; }
        public string Winner { get; set; }
        public string[] Slots { get; set; }

        // just an ugly shortcut...
        IEnumerator<GameListEntry> IEnumerable<GameListEntry>.GetEnumerator() {
            yield return this;
        }
        IEnumerator IEnumerable.GetEnumerator() {
            yield return this;
        }
    }
}
