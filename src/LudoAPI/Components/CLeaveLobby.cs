using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CLeaveLobby : ILeaveLobby
    {
        private readonly ILudoService ludoService;

        public CLeaveLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public ErrorCode TryLeaveLobby(string userId, string gameId)
        {
            return ludoService.LeaveLobby(userId: userId, gameId: gameId);
            //Placeholder:
            //return gameId != "test"; // just for experimentation
        }
    }
}
