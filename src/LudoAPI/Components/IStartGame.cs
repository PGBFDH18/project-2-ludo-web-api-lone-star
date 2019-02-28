using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IStartGame
    {
        ErrorCode TryStartGame(string gameId);
    }
}