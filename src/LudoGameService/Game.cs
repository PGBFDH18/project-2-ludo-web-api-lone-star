using Ludo.API.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Ludo.API.Service
{
    public partial class Game
    {
        public Game(SetupPhase lobby)
        {
            if (lobby != null)
                _phase = lobby;
        }

        public string Winner => _phase.Winner;

        // WARNING: does NOT check that the user exists!
        internal Error TryAddUser(string userId, out int slotIndex)
        {
            // TODO: LobbyAccess rules
            slotIndex = -1;
            var setup = _phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            return setup.TryAddUser(userId, out slotIndex); // 00,01,03,04
        }

        public bool UserIsPlayer(string userId)
            => _phase.Slots?.Contains(userId) == true;

        // lobby state data
        // active state data
        // finished state data
        public IGamePhase Phase => _phase;
        private IGamePhase _phase = TransitionPhase.Creating;

        public Error TryStartGame()
        {
            var setup = _phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            var err = setup.TryFinalLock();
            if (err)
                return err;
            var trans = new TransitionPhase(GamePhase.starting, setup);
            //since only once thread can successfully call TryFinalLock, we don't need a CompareExchange here:
            Volatile.Write(ref _phase, trans); // (likely overkill, but it occurs seldom so just in case...)

            // ------------ TODO: do we want a countdown before the game starts?
            // currently we just start "instantly"...

            // ------ Setup game, bots, and listeners, then call Start! ------
            var ingame = new IngamePhase(trans.Slots);
            ingame.WinnerDeclared += Ingame_WinnerDeclared;
            // TODO: bots

            Volatile.Write(ref _phase, trans); // (likely overkill, but it occurs seldom so just in case...)
            ingame.Start();

            return Error.Codes.E00NoError;
        }

        private void Ingame_WinnerDeclared(IngamePhase sender)
        {
            // assuming we have handled the logic session correctly...
            // ...this should only called from one thread once (race free).
            sender.WinnerDeclared -= Ingame_WinnerDeclared; // <- important!
            Finish(sender);
        }

        private void Finish(IngamePhase ingame)
        {
            // (kind of redundant but, whatever... keeps things consistent)
            var trans = new TransitionPhase(GamePhase.ending, ingame);
            Volatile.Write(ref _phase, trans);

            // -- announce that the game is / has ended (in a separate thread?)

            var finished = new FinishedPhase(ingame.Winner, ingame.Slots);
            Volatile.Write(ref _phase, finished);
        }

        // thread-safe, lock-free, performs roll-back on to_factory failure.
        // Intent: I dont want to call the factory before I can successfully assign it.
        // (I could have used a lock, but I didn't...)
        private bool TryTransition(GamePhase from, IGamePhase via, Func<IGamePhase> to_factory)
        {
            // null-checks + transition direction check + via.State is an even (i.e. is transitional) check.
            Debug.Assert(via != null && to_factory != null && (via.Phase - 1 == from) && ((int)via.Phase & 1) == 0);
            var old = _phase;
            if (old.Phase == from && ReferenceEquals(old, Interlocked.CompareExchange(ref _phase, via, old)))
            {
                // we have successfully entered the transition state (i.e. via)!
                // where we have succeeded all others shall fail!
                // ...which is a fancy way of saying only one thread can be in this block at a time.
                // simply put: for as long as _game is in the via state, we have "lock".

                try
                {
                    //var data = old.FinalLockDown(); // TODO <-----------------------------------
                    var next = to_factory();
                    if (next != null && (via.Phase + 1 == next.Phase))
                    {
                        _phase = next; // success, and "lock" released.
                        return true;
                    }
                    return false; // (false will be returned *after* finally has executed.)
                }
                finally
                {
                    if (ReferenceEquals(via, _phase)) // we failed to transition to next?
                        _phase = old; // performing roll-back, and "lock" released.
                }
            }
            return false; // some other thread beat us to the punch.
        }
    }
}
