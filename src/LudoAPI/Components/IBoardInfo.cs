using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IBoardInfo
    {
        bool TryGetBoardInfo(int length, out Models.BoardInfo info);
    }
}