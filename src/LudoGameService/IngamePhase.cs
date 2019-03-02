using System.Linq;

namespace Ludo.GameService
{
    public class IngamePhase : IGamePhase
    {
        private readonly GameLogic.ISession session;
        private readonly ISlotArray slots;

        public IngamePhase(ISlotArray slots, int startingSlot = -1)
        {
            this.slots = new SlotArray(slots); // <-- makes a copy
            session = GameLogic.SessionFactory.New(playerCount: slots.PlayerCount);
        }

        internal bool Start()
            => session.Start();

        #region --- IGameStateSession ---
        GameLifecycle IGamePhase.Phase => GameLifecycle.ingame;

        SetupPhase IGamePhase.Setup => null;

        IngamePhase IGamePhase.Ingame => this;

        FinishedPhase IGamePhase.Finished => null;
        
        ISlotArray IGamePhase.Slots => slots;
        #endregion
    }
}
