using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CStartGame : IStartGame
    {
        private readonly ILudoService ludoService;

        public CStartGame(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public ErrorCode TryStartGame(string gameId)
            => ludoService.StartGame(gameId);
    }

    public class CStartGameMock : IStartGame
    {
        public ErrorCode TryStartGame(string gameId)
            => gameId == "test"
            ? ErrorCodes.E01GameNotFound
            : ErrorCodes.E00NoError;
    }
}
