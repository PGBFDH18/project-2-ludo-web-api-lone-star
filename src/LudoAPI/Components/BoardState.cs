using Ludo.GameService;
using Ludo.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ludo.WebAPI.Components
{
    class BoardState : IBoardState
    {
        private readonly ILudoService ludoService;

        public BoardState(ILudoService ludoService)
        {
            this.ludoService = ludoService;
        }

        public ErrorCode TryGetBoardState(string gameId, out Models.BoardState bstate)
        {
            bstate = default;
            var game = ludoService.Games.FirstOrDefault((kvp) => kvp.Key.Encoded == gameId).Value;
            if (game == null)
                return new ErrorCode { Code = ErrorCodes.Err01GameNotFound };

            //TODO
            throw new NotImplementedException();
        }
    }

    class BoardStateMock
    {
        public bool TryGetBoardState(string gameId, out Models.BoardState bstate, out ErrorCode whyNot)
        {
            //TODO
            bstate = new Models.BoardState { };
            whyNot = null;
            return gameId != "test";
        }
    }
}
