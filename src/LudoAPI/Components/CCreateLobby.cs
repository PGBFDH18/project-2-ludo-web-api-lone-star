using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CCreateLobby : ICreateLobby
    {
        private readonly ILudoService ludoService;

        public CCreateLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public int MinSlots => GameLogic.SessionFactory.MinPlayers;
        public int MaxSlots => GameLogic.SessionFactory.MaxPlayers;

        public ErrorCode TryCreateLobby(string userId, int? slotCount, Models.LobbyAccess access, out string gameId)
            => ludoService.CreateLobby(userId, slotCount ?? MaxSlots, (GameService.LobbyAccess)access, out gameId);
            //Placeholder:
            //gameId = $"placeholder({userId})";
            //return userId != "test"; // just for experimentation
    }
}
