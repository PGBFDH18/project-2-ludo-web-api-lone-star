using System.Linq;

namespace Ludo.GameService
{
    public class IngameSession : IGamePhase//, ISharedGP
    {
        private readonly GameLogic.ISession session;
        private readonly IUserIdArray players;

        public IngameSession(int playerCount, IUserIdArray players)
        {
            this.players = new UserArray(players);
            session = GameLogic.SessionFactory.New(playerCount: playerCount);
        }

        #region --- IGameStateSession ---
        GameLifecycle IGamePhase.State => GameLifecycle.ingame;

        SetupPhase IGamePhase.Setup => null;

        IngameSession IGamePhase.Ingame => this;

        FinishedPhase IGamePhase.Finished => null;

        //ISharedGP IGamePhase.Shared => this;

        IUserIdArray IGamePhase.Slots => players;
        #endregion
    }
}
