using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface ICreateLobby
    {
        int MinSlots { get; }
        int MaxSlots { get; }

        // param: slotCount defaults to MaxSlots
        Error TryCreateLobby(string userId, int? slotCount, LobbyAccess access, out string gameId);
    }
}