namespace Ludo.GameService
{

    public partial class Game
    {
        // helps with transition phases.
        private class TransitionPhase : IGamePhase
        {
            public static readonly IGamePhase Creating = new TransitionPhase(GameLifecycle.creating, null);

            internal TransitionPhase(GameLifecycle newPhase, IGamePhase oldPhase)
            {
                _newPhase = newPhase;
                _oldPhase = oldPhase;
            }

            private readonly GameLifecycle _newPhase;
            private readonly IGamePhase _oldPhase;

            public GameLifecycle Phase => _newPhase;
            public ISlotArray Slots => _oldPhase?.Slots;

            SetupPhase IGamePhase.Setup => _oldPhase?.Setup;
            IngamePhase IGamePhase.Ingame => _oldPhase?.Ingame;
            FinishedPhase IGamePhase.Finished => _oldPhase?.Finished;
        }
    }
}
