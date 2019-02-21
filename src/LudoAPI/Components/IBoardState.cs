using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IBoardState
    {
        ErrorCode TryGetBoardState(string gameId, out Models.BoardState bstate);
    }
}