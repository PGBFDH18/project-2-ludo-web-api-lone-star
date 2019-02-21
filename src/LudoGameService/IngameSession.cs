using System.Linq;

namespace Ludo.GameService
{
    public class IngameSession : IGameStateSession, ISharedGSS
    {
        private readonly GameLogic.ISession session;
        private readonly IUserIdArray players;

        public IngameSession(int playerCount, IUserIdArray players)
        {
            this.players = new UserArray(players);
            session = GameLogic.SessionFactory.New(playerCount: playerCount);
        }

        IUserIdArray ISharedGSS.Slots => players;

        #region --- IGameStateSession ---
        GameState IGameStateSession.State => GameState.ingame;

        SetupSession IGameStateSession.Setup => null;

        IngameSession IGameStateSession.Ingame => this;

        FinishedSession IGameStateSession.Finished => null;

        ISharedGSS IGameStateSession.Shared => this;
        #endregion
    }
}
