namespace Ludo.GameService
{
    public class FinishedPhase : IGamePhase//, ISharedGP
    {
        private readonly ISlotArray players;

        public FinishedPhase(byte winner, ISlotArray players)
        {
            WinnerSlot = winner;
            this.players = new SlotArray(players);
        }

        // index of the winning slot.
        public byte WinnerSlot { get; }

        public string WinnerId => players[WinnerSlot];

        #region --- IGameStateSession ---
        GameLifecycle IGamePhase.Phase => GameLifecycle.finished;

        SetupPhase IGamePhase.Setup => null;

        IngamePhase IGamePhase.Ingame => null;

        FinishedPhase IGamePhase.Finished => this;

        //ISharedGP IGamePhase.Shared => this;

        ISlotArray IGamePhase.Slots => players;
        #endregion
    }
}
