using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IMovePiece
    {
        Error MovePiece(string gameId, int slot, int piece);
    }
}