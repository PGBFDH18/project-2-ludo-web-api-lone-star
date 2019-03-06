using Ludo.API.Models;

namespace Ludo.API.Service
{

    public partial class Game
    {
        // helps with transition phases.
        private class TransitionPhase : IGamePhase
        {
            public static readonly IGamePhase Creating = new TransitionPhase(GamePhase.creating, null);

            internal TransitionPhase(GamePhase newPhase, IGamePhase oldPhase)
            {
                _newPhase = newPhase;
                _oldPhase = oldPhase;
            }

            private readonly GamePhase _newPhase;
            private readonly IGamePhase _oldPhase;

            public GamePhase Phase => _newPhase;
            public ISlotArray Slots => _oldPhase?.Slots;
            public string Winner => _oldPhase?.Winner;

            SetupPhase IGamePhase.Setup => _oldPhase?.Setup;
            IngamePhase IGamePhase.Ingame => _oldPhase?.Ingame;
            FinishedPhase IGamePhase.Finished => _oldPhase?.Finished;
        }
    }
}
