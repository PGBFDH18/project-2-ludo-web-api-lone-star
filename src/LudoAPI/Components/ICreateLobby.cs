using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface ICreateLobby
    {
        int MinSlots { get; }
        int MaxSlots { get; }

        // param: slotCount defaults to MaxSlots
        ErrorCode TryCreateLobby(string userId, int? slotCount, LobbyAccess access, out string gameId);
    }
}