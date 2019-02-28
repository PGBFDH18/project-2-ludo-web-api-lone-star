namespace Ludo.GameService
{
    public interface ILudoService
    {
        GameStorage Games { get; }
        UserStorage Users { get; }

        Error CreateLobby(string userId, int slots, LobbyAccess access, out string gameId);
        Error JoinLobby(string userId, string gameId, out int slot);
        Error LeaveLobby(string userId, string gameId);
        Error GetPlayerReady(string gameId, int slot, out UserReady userReady);
        // Sets user/player AND ready status for a slot.
        Error SetSlotReady(string gameId, int slot, UserReady userReady);
        Error UnSlotUser(string gameId, string userId);
        Error ClaimSlot(string gameId, int slot, string userId);
    }
}