namespace Ludo.API.Service.Components
{
    public interface IIsKnown
    {
        bool GameId(string gameId);
        bool UserId(string userId);
    }
}
