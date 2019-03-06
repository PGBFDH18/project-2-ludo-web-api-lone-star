using Ludo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.API.Service.Components
{
    public class CListLobbies : IListLobbies
    {
        private readonly ILudoService ludoService;

        public CListLobbies(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public IEnumerable<LobbyListEntry> ListLobbies(ShowLobby show, string[] users)//, out IEnumerable<LobbyListEntry> result)
        {
            // a bit of a Linq mess here...
            return ludoService.Games
                .Where(kvp => kvp.Value.Phase.Phase == GamePhase.setup
                && kvp.Value.Phase.Setup != null
                && (show == ShowLobby.all
                || (show == ShowLobby.full && kvp.Value.Phase.Setup.Data.OpenCount == 0)
                || (show == ShowLobby.open && kvp.Value.Phase.Setup.Data.OpenCount > 0)
                || (show == ShowLobby.penultimate && kvp.Value.Phase.Setup.Data.OpenCount == 1))
                && (users == null || users.Length == 0 || kvp.Value.Phase.Setup.Data.Any(u => users.Contains(u))))
                .Select(kvp => new LobbyListEntry
                {
                    GameId = kvp.Key.Encoded,
                    Slots = kvp.Value.Phase.Setup.Data.ToArray(),
                    Others = kvp.Value.Phase.Setup.Data.Others,
                    Access = LobbyAccess.@public // TODO: LobbyAccess
                });
            //Placeholder:
            //result = new LobbyListEntry[] {
            //    new LobbyListEntry {
            //        GameId = show.ToString(),
            //        Access = ModelState.IsValid ? LobbyAccess.@public : LobbyAccess.unlisted,
            //        Slots = new string[] {
            //            "placeholder",
            //            users?.Length >= 1 ? users[0] : null, // for experimentation
            //            users?.Length >= 2 ? users[1] : null,
            //            users?.Length >= 3 ? users[1] : null,
            //}   }   };
            //return !users.Contains("test");
        }
    }
}
