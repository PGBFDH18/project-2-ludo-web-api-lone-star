using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CLeaveLobby : ILeaveLobby
    {
        private readonly ILudoService ludoService;

        public CLeaveLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryLeaveLobby(string userId, string gameId)
        {
            return ludoService.LeaveLobby(userId: userId, gameId: gameId);
            //Placeholder:
            //return gameId != "test"; // just for experimentation
        }
    }
}
