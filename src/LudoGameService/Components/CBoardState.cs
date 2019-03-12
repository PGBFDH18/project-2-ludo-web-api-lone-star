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
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err)
                return err;
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
