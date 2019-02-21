namespace Ludo.GameService
{
    public class FinishedPhase : IGamePhase//, ISharedGP
    {
        private readonly IUserIdArray players;

        public FinishedPhase(byte winner, IUserIdArray players)
        {
            WinnerSlot = winner;
            this.players = new UserArray(players);
        }

        // index of the winning slot.
        public byte WinnerSlot { get; }

        public string WinnerId => players[WinnerSlot];

        #region --- IGameStateSession ---
        GameLifecycle IGamePhase.State => GameLifecycle.finished;

        SetupPhase IGamePhase.Setup => null;

        IngameSession IGamePhase.Ingame => null;

        FinishedPhase IGamePhase.Finished => this;

        //ISharedGP IGamePhase.Shared => this;

        IUserIdArray IGamePhase.Slots => players;
        #endregion
    }
}
