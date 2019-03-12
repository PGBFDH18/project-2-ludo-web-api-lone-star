using Ludo.API.Models;

namespace Ludo.API.Service
{
    public class FinishedPhase : IGamePhase
    {
        private readonly ISlotArray slots;

        public FinishedPhase(int winner, ISlotArray slots)
        {
            WinnerSlot = winner;
            this.slots = new SlotArray(slots);
        }

        // index of the winning slot.
        public int WinnerSlot { get; }

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
