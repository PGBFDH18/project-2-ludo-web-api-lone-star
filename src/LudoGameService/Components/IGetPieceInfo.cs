using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetPieceInfo
    {
        Error GetPieceInfo(string gameId, int slot, int piece, out PieceInfo pieceInfo);
    }
}