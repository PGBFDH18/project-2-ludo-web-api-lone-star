using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CCreateLobby : ICreateLobby
    {
        private readonly ILudoService ludoService;

        public CCreateLobby(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public int MinSlots => GameLogic.SessionFactory.MinPlayers;
        public int MaxSlots => GameLogic.SessionFactory.MaxPlayers;

        public Error TryCreateLobby(string userId, int? slotCount, LobbyAccess access, out string gameId)
            => ludoService.CreateLobby(userId, slotCount ?? MaxSlots, access, out gameId);
            //Placeholder:
            //gameId = $"placeholder({userId})";
            //return userId != "test"; // just for experimentation
    }
}
