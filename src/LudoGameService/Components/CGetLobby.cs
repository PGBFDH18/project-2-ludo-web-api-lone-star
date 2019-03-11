using Ludo.API.Models;
using System.Linq;

namespace Ludo.API.Service.Components
{
    public class CGetLobby : IGetLobby
    {
        private readonly ILudoService ludoService;

        public CGetLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public bool TryGetLobby(string gameId, out LobbyInfo lobbyInfo)
        {
            lobbyInfo = null;
            var game = ludoService.Games.TryGet(Id.Partial(gameId))?.Phase;
            if (game == null)
                return false;
            lobbyInfo = new LobbyInfo
            {
                Access = game.Setup?.Access ?? LobbyAccess.@public,
                State = game.Phase,
                Others = game.Setup?.Data.Others,
                Slots = (game.Setup == null
                ? game.Slots?.Select(u => new PlayerReady { UserId = u })
                : game.Setup.Data.Slots.Select(UserReady.ToModel)
                ).ToArray(),
                Reservations = null, // <-- TODO
            };
            return true;
            //Placeholder:
            //lobbyInfo = new LobbyInfo {
            //    Access = LobbyAccess.@public,
            //    State = Models.GameState.setup,
            //    Slots = new PlayerReady[] {
            //        new PlayerReady{
            //            UserId = $"placeholder1 ({gameId})",
            //            Ready = true,
            //        },
            //        new PlayerReady{
            //            UserId = $"placeholder2 ({gameId})",
            //            Ready = false,
            //        },
            //    },
            //    Reservations = new LobbyReservation[] {
            //        new LobbyReservation { Player = "olle", Slot = 0, Strict = false },
            //        new LobbyReservation { Player = "pelle", Slot = 0, Strict = false },
            //    },
            //};
            //return gameId != "test"; // just for experimentation
        }
    }
}
