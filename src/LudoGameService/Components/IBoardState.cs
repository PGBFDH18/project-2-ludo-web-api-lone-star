using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IBoardState
    {
        Error TryGetBoardState(string gameId, out Models.BoardState bstate);
    }
}