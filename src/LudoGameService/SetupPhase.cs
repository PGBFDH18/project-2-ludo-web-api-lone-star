using System;
using System.Linq;
using System.Threading;

namespace Ludo.GameService
{
    public partial class SetupPhase : IGamePhase//, ISharedGP
    {
        public static readonly int MaxUsers = 8;

        private volatile PhaseData data;

        // ASSUMES ownerId is a valid userId !!!
        public SetupPhase(string ownerId, int slotCount, LobbyAccess access)
        {
            GameLogic.SessionFactory.Validate.PlayerCount(slotCount);
            data = new PhaseData(new UserReady[slotCount].Modify(0, new UserReady(ownerId)), Array.Empty<string>());
            Access = access;
        }

        // When we transtion out from a phase and into the next in a game's lifecycle,
        // we need a mechanism to ensure that all unfinished modifications fail.
        // The Final Lock Down grabs the data from this phase one last time,
        // removing it in the process, which signals to any thread still attempting
        // modifications that their efforts are futile and must fail.
        internal ISetupPhaseData FinalLockDown()
            => Interlocked.Exchange(ref data, null);

        public LobbyAccess Access { get; private set; }

        public ISetupPhaseData Data => data;

        public Error TryAddSlot()
        {
            PhaseData @new, old;
            do {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                if (old.IsEmpty)
                    // game is in the process of being removed, hence "not found".
                    return ErrorCodes.E01GameNotFound;
                @new = old.TryAddSlot();
                if (@new == null)
                    return ErrorCodes.E13MaxSlotsReached;
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            return ErrorCodes.E00NoError;
        }

        // moves a user from their slot to Others
        // returns success if the user is already in others
        // WARNING: does NOT check that user exists!
        internal Error TryUnSlot(string userId)
        {
            PhaseData @new, old;
            do
            {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                @new = old.TryMoveFromSlotToOthers(userId);
                if (@new == null)
                {
                    if (old.TryFindOther(userId, out _))
                        // user already in Others
                        return ErrorCodes.E00NoError;
                    if (old.IsEmpty)
                        // game is in the process of being removed, hence "not found".
                        return ErrorCodes.E01GameNotFound;
                    return ErrorCodes.E11UserNotInLobby;
                }
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            return ErrorCodes.E00NoError;
        }

        // WARNING: does NOT check that user exists!
        internal Error TryClaimSlot(int slot, string userId, Func<string, bool> allowEvict = null)
        {
            PhaseData @new, old;
            do
            {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                if (old.IsEmpty)
                    // game is in the process of being removed, hence "not found".
                    return ErrorCodes.E01GameNotFound;
                if (unchecked((uint)slot >= (uint)old.SlotCount))
                    return ErrorCodes.E10InvalidSlotIndex;
                string occupant = old[slot].UserId;
                if (occupant != null && (allowEvict == null || !allowEvict(occupant)))
                    return ErrorCodes.E09CannotEvictSlotOccupant;
                @new = old.TryMoveToSlot(slot, userId, true);
                if (@new == old) // no change
                    return ErrorCodes.E00NoError;
                if (@new == null)
                {
                    if (old.IsEmpty)
                        // game is in the process of being removed, hence "not found".
                        return ErrorCodes.E01GameNotFound;
                    else
                        return ErrorCodes.E11UserNotInLobby;
                }
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            return ErrorCodes.E00NoError;
        }
        
        // WARNING: does NOT check that the user exists!
        internal Error TrySetSlotReady(int slot, UserReady ur)
        {
            PhaseData @new, old;
            do
            {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                if (unchecked((uint)slot >= (uint)old.SlotCount))
                    return ErrorCodes.E10InvalidSlotIndex;
                @new = old.TrySetSlotReady(slot, ur);
                if (@new == old) // no change
                    return ErrorCodes.E00NoError;
                if (@new == null)
                {
                    if (old.IsEmpty)
                        // game is in the process of being removed, hence "not found".
                        return ErrorCodes.E01GameNotFound;
                    else
                        return ErrorCodes.E12UserIdMismatch;
                }
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            return ErrorCodes.E00NoError;
        }

        // also returns true if the user was already present
        // WARNING: does NOT check that the user exists!
        internal Error TryAddUser(string userId, out int slot)
        {
            slot = -1;
            PhaseData @new, old;
            do
            {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                if (old.IsEmpty)
                    // game is in the process of being removed, hence "not found".
                    return ErrorCodes.E01GameNotFound;
                @new = old.TryAddUser(userId, out slot);
                if (@new == null)
                    return ErrorCodes.E04LobbyIsFull;
                if (@new == old)
                    return ErrorCodes.E00NoError; // no change
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            return ErrorCodes.E00NoError;
        }

        // WARNING: does NOT check that the user exists!
        internal Error TryLeaveLobby(string userId, out bool wasLastUser)
        {
            wasLastUser = false;
            PhaseData old, @new;
            do
            {
                old = data;
                if (old == null)
                    // final locked: game is being transitioned to the next phase.
                    return ErrorCodes.E03NotInSetupPhase;
                if (old.IsEmpty)
                    // game is in the process of being removed, hence "not found".
                    return ErrorCodes.E01GameNotFound;
                @new = old.LeaveLobby(userId); // <- returns null if no modification took place.
                if (@new == null)
                    return ErrorCodes.E00NoError;
            }
            while (Interlocked.CompareExchange(ref data, @new, old) != old);
            wasLastUser = @new.IsEmpty;
            return ErrorCodes.E00NoError;
            //do @new = (old = data)?.LeaveLobby(userId);
            //while (@new != null && Interlocked.CompareExchange(ref data, @new, old) != old);
            //isEmpty = @new?.IsEmpty == true;
            //return old != null;
        }

        #region --- IGameStateSession ---
        GameLifecycle IGamePhase.State => GameLifecycle.setup;

        SetupPhase IGamePhase.Setup => this;

        IngameSession IGamePhase.Ingame => null;

        FinishedPhase IGamePhase.Finished => null;

        //ISharedGP IGamePhase.Shared => this;

        IUserIdArray IGamePhase.Slots => data;
        #endregion
    }
}
