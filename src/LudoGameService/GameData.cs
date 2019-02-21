using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Ludo.GameService
{
    // TODO: add data...
    public class GameData
    {
        // lobby state data
        // active state data
        // finished state data

        public GameData(SetupSession lobby)
        {
            if (lobby != null)
                _game = lobby;
        }

        public GameState GameState => Game.State;

        public string Winner => _game.Finished?.WinnerId;

        public Error TryAddUser(string userId, out int slotIndex)
        {
            // TODO: LobbyAccess rules
            slotIndex = -1;
            var g = _game.Setup;
            if (g == null)
                return new Error(ErrorCodes.Err03NotInSetupState);
            if (!g.TryAddUser(userId, out slotIndex))
                return new Error(ErrorCodes.Err04LobbyIsFull);
            if (!(_game == g || UserIsPlayer(userId)))
                return new Error(ErrorCodes.Err03NotInSetupState);
            return null;
        }

        public bool UserIsPlayer(string userId)
            => _game.Shared?.Slots.Contains(userId) == true;

        public SetupSession TryGetSetup => _game.Setup;

        internal IGameStateSession Game => _game;
        private volatile IGameStateSession _game = TransitionState.Creating;

        // thread-safe, lock-free, performs roll-back on to_factory failure.
        // Intent: I dont want to call the factory before I can successfully assign it.
        // (I could have used a lock, but I didn't...)
        private bool TryTransition(GameState from, IGameStateSession via, Func<IGameStateSession> to_factory)
        {
            // null-checks + transition direction check + via.State is an even (i.e. is transitional) check.
            Debug.Assert(via != null && to_factory != null && (via.State - 1 == from) && ((int)via.State & 1) == 0);
            var old = _game;
            if (old.State == from && ReferenceEquals(old, Interlocked.CompareExchange(ref _game, via, old)))
            {
                // we have successfully entered the transition state (i.e. via)!
                // where we have succeeded all others shall fail!
                // ...which is a fancy way of saying only one thread can be in this block at a time.
                // simply put: for as long as _game is in the via state, we have "lock".

                try
                {
                    var next = to_factory();
                    if (next != null && (via.State + 1 == next.State))
                    {
                        _game = next; // success, and "lock" released.
                        return true;
                    }
                    return false; // (false will be returned *after* finally has executed.)
                }
                finally
                {
                    if (ReferenceEquals(via, _game)) // we failed to transition to next?
                        _game = old; // performing roll-back, and "lock" released.
                }
            }
            return false; // some other thread beat us to the punch.
        }

        // provides transition states.
        private class TransitionState : IGameStateSession
        {
            public static readonly IGameStateSession Creating = new TransitionState(GameState.creating);
            public static readonly IGameStateSession Starting = new TransitionState(GameState.starting);
            public static readonly IGameStateSession Ending = new TransitionState(GameState.ending);

            private TransitionState(GameState state) => _state = state;
            private readonly GameState _state;
            GameState IGameStateSession.State => _state;
            SetupSession IGameStateSession.Setup => null;
            IngameSession IGameStateSession.Ingame => null;
            FinishedSession IGameStateSession.Finished => null;
            ISharedGSS IGameStateSession.Shared => null;
        }
    }
}
