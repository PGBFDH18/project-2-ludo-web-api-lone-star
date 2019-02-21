namespace Ludo.GameService
{
    public interface ILudoService
    {
        GameStorage Games { get; }
        UserStorage Users { get; }

        Error TryCreateLobby(string userId, int slots, out string gameId);
    }
}