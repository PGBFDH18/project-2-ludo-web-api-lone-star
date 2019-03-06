using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IBoardInfo
    {
        bool TryGetBoardInfo(int length, out Models.BoardInfo info);
    }
}