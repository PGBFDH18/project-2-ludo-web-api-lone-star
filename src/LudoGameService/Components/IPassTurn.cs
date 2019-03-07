using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IPassTurn
    {
        Error PassTurn(string gameId, int slot);
    }
}