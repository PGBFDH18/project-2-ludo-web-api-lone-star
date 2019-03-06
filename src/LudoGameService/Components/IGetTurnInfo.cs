using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetTurnInfo
    {
        Error GetTurnInfo(string gameId, int slot, out TurnInfo turnInfo);
    }
}