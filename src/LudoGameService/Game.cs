﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Ludo.GameService
{
    // TODO: add data...
    public partial class Game
    {
        // lobby state data
        // active state data
        // finished state data

        public Game(SetupPhase lobby)
        {
            if (lobby != null)
                _phase = lobby;
        }

        public string Winner => _phase.Finished?.WinnerId;

        // WARNING: does NOT check that the user exists!
        internal Error TryAddUser(string userId, out int slotIndex)
        {
            // TODO: LobbyAccess rules
            slotIndex = -1;
            var setup = _phase.Setup;
            if (setup == null)
                return ErrorCodes.E03NotInSetupPhase;
            return setup.TryAddUser(userId, out slotIndex); // 00,01,03,04
        }

        public bool UserIsPlayer(string userId)
            => _phase.Slots?.Contains(userId) == true;

        public IGamePhase Phase => _phase;
        private volatile IGamePhase _phase = TransitionPhase.Creating;

        // thread-safe, lock-free, performs roll-back on to_factory failure.
        // Intent: I dont want to call the factory before I can successfully assign it.
        // (I could have used a lock, but I didn't...)
        private bool TryTransition(GameLifecycle from, IGamePhase via, Func<IGamePhase> to_factory)
        {
            // null-checks + transition direction check + via.State is an even (i.e. is transitional) check.
            Debug.Assert(via != null && to_factory != null && (via.State - 1 == from) && ((int)via.State & 1) == 0);
            var old = _phase;
            if (old.State == from && ReferenceEquals(old, Interlocked.CompareExchange(ref _phase, via, old)))
            {
                // we have successfully entered the transition state (i.e. via)!
                // where we have succeeded all others shall fail!
                // ...which is a fancy way of saying only one thread can be in this block at a time.
                // simply put: for as long as _game is in the via state, we have "lock".

                try
                {
                    //var data = old.FinalLockDown(); // TODO <-----------------------------------
                    var next = to_factory();
                    if (next != null && (via.State + 1 == next.State))
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
