using Ludo.GameService;
using Ludo.WebAPI.Models;
using System.Linq;

namespace Ludo.WebAPI.Components
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
                return false; //new ErrorCode { Code = ErrorCodes.Err01GameNotFound };
            lobbyInfo = new LobbyInfo
            {
                Access = (Models.LobbyAccess)(game.Setup?.Access ?? GameService.LobbyAccess.@public),
                State = (GameState)game.State,
                Others = game.Setup?.Data.Others,
                Slots = (game.Setup == null
                ? game.Slots?.Select(u => new PlayerReady { UserId = u })
                : game.Setup.Data.Slots.Select(ur => new PlayerReady { UserId = ur.UserId, Ready = ur.IsReady })
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
