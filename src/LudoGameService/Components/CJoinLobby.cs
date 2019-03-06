using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CJoinLobby : IJoinLobby
    {
        private readonly ILudoService ludoService;

        public CJoinLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryJoinLobby(string gameId, string userId, out int slot)
        {
            return ludoService.JoinLobby(userId, gameId, out slot);
            //Placeholder:
            //slot = 0;
            //return gameId != "test"; // just for experimentation
        }
    }
}
