namespace Ludo.WebAPI.Components
{
    public interface IIsKnown
    {
        bool GameId(string gameId);
        bool UserId(string userId);
    }
}
