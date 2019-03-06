using Ludo.API.Service.Extensions;

namespace Ludo.API.Service.Components
{
    public class CBoardInfo : IBoardInfo
    {
        public bool TryGetBoardInfo(int length, out Models.BoardInfo info)
        {
            if (GameLogic.BoardInfo.IsValid.Length(length))
            {
                info = (new GameLogic.BoardInfo(length)).ToModel();
                return true;
            }
            else
            {
                info = default;
                return false;
            }
        }
    }
}
