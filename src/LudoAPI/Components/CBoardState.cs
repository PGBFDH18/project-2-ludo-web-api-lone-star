using Ludo.GameService;
using Ludo.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.WebAPI.Components
{
    class CBoardState : IBoardState
    {
        private readonly ILudoService ludoService;

        public CBoardState(ILudoService ludoService)
        {
            this.ludoService = ludoService;
        }

        public ErrorCode TryGetBoardState(string gameId, out BoardState bstate)
        {
            bstate = default;
            var game = ludoService.Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return ErrorCodes.E01GameNotFound;
            var ing = game.Phase.Ingame;
            if (ing == null)
                return ErrorCodes.E07NotInGamePhase;

            //TODO
            throw new NotImplementedException();
        }
    }

    class CBoardStateMock : IBoardState
    {
        public ErrorCode TryGetBoardState(string gameId, out BoardState bstate)
        {
            bstate = new BoardState { };
            return gameId == "test" ? ErrorCodes.E01GameNotFound : ErrorCodes.E00NoError;
        }
    }
}
