using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CBoardState : IBoardState
    {
        private readonly ILudoService ludoService;

        public CBoardState(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryGetBoardState(string gameId, out BoardState bstate)
        {
            bstate = default;
            var g = ludoService.Games.TryGet(Id.Partial(gameId));
            if (g == null)
                return Error.Codes.E01GameNotFound;
            var ingame = g.Phase.Ingame;
            if (ingame == null)
                return Error.Codes.E07NotInGamePhase;
            bstate = ingame.GetBoardState();
            return Error.Codes.E00NoError;
        }
    }

    class CBoardStateMock : IBoardState
    {
        public Error TryGetBoardState(string gameId, out BoardState bstate)
        {
            bstate = new BoardState { };
            return gameId == "test" ? Error.Codes.E01GameNotFound : Error.Codes.E00NoError;
        }
    }
}
