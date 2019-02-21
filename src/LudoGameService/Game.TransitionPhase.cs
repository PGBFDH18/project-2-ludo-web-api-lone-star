namespace Ludo.GameService
{

    public partial class Game
    {
        // provides transition states.
        private class TransitionPhase : IGamePhase
        {
            public static readonly IGamePhase Creating = new TransitionPhase(GameLifecycle.creating);
            public static readonly IGamePhase Starting = new TransitionPhase(GameLifecycle.starting);
            public static readonly IGamePhase Ending = new TransitionPhase(GameLifecycle.ending);

            private TransitionPhase(GameLifecycle state) => _state = state;
            private readonly GameLifecycle _state;
            GameLifecycle IGamePhase.State => _state;
            SetupPhase IGamePhase.Setup => null;
            IngameSession IGamePhase.Ingame => null;
            FinishedPhase IGamePhase.Finished => null;
            //ISharedGP IGamePhase.Shared => null;
            IUserIdArray IGamePhase.Slots => null;
        }
    }
}
