using Ludo.API.Models;

namespace Ludo.API.Service
{
    public interface ILudoService
    {
        GameStorage Games { get; }
        UserStorage Users { get; }

        Error CreateLobby(string userId, int slots, LobbyAccess access, out string gameId);
        Error JoinLobby(string userId, string gameId, out int slot);
        Error LeaveLobby(string userId, string gameId);
        Error GetPlayerReady(string gameId, int slot, out UserReady userReady);
        Error SetSlotReady(string gameId, int slot, UserReady userReady);
        Error UnSlotUser(string gameId, string userId);
        Error ClaimSlot(string gameId, int slot, string userId);
        Error StartGame(string gameId);
        Error Concede(string gameId, string userId); // TODO: auth!
    }
}