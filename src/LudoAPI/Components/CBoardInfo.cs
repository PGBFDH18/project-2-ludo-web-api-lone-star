namespace Ludo.WebAPI.Components
{
    public class CBoardInfo : IBoardInfo
    {
        public bool TryGetBoardInfo(int length, out Models.BoardInfo info)
        {
            if (GameLogic.BoardInfo.IsValid.Length(length))
            {
                info = new GameLogic.BoardInfo(length); // implicit-cast
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
