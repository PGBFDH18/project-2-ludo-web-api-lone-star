using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CStartGame : IStartGame
    {
        private readonly ILudoService ludoService;

        public CStartGame(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryStartGame(string gameId)
            => ludoService.StartGame(gameId);
    }

    public class CStartGameMock : IStartGame
    {
        public Error TryStartGame(string gameId)
            => gameId == "test"
            ? Error.Codes.E01GameNotFound
            : Error.Codes.E00NoError;
    }
}
