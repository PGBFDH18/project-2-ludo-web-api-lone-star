using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CStartGame : IStartGame
    {
        public ErrorCode TryStartGame(string gameId)
        {
            throw new System.NotImplementedException(); // TODO <--------------
        }
    }

    public class CStartGameMock : IStartGame
    {
        public ErrorCode TryStartGame(string gameId)
            => gameId == "test"
            ? ErrorCodes.E01GameNotFound
            : ErrorCodes.E00NoError;
    }
}
