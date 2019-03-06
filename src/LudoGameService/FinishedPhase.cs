using Ludo.API.Models;

namespace Ludo.API.Service
{
    public class FinishedPhase : IGamePhase
    {
        private readonly ISlotArray slots;

        public FinishedPhase(byte winner, ISlotArray players)
        {
            WinnerSlot = winner;
            this.slots = new SlotArray(players);
        }

        // index of the winning slot.
        public byte WinnerSlot { get; }

        public string Winner => slots[WinnerSlot];

        #region --- IGameStateSession ---
        GamePhase IGamePhase.Phase => GamePhase.finished;

        SetupPhase IGamePhase.Setup => null;

        IngamePhase IGamePhase.Ingame => null;

        FinishedPhase IGamePhase.Finished => this;

        ISlotArray IGamePhase.Slots => slots;
        #endregion
    }
}
