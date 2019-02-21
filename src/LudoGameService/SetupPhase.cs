using System;
using System.Threading;

namespace Ludo.GameService
{
    public partial class SetupPhase : IGamePhase//, ISharedGP
    {
        public static readonly int MaxOthers = 8;

        private volatile PhaseData data;

        // ASSUMES ownerId is a valid userId !!!
        public SetupPhase(string ownerId, int slotCount)
        {
            GameLogic.SessionFactory.Validate.PlayerCount(slotCount);
            data = new PhaseData(new UserReady[slotCount].Modify(0, new UserReady(ownerId)), Array.Empty<string>());
        }

        // When we transtion out from a phase and into the next in a game's lifecycle,
        // we need a mechanism to ensure that all unfinished modifications fail.
        // The Final Lock Down grabs the data from this phase one last time,
        // removing it in the process, which signals to any thread still attempting
        // modifications that their efforts are futile and must fail.
        internal ISetupPhaseData FinalLockDown()
            => Interlocked.Exchange(ref data, null);

        public ISetupPhaseData Data => data;

        public bool TryAddSlot()
        {
            var prev = data;
            if (prev == null)
                return false;
            var grown = prev;
            int len = prev.SlotCount;
            do grown = prev.TryAddSlot(); // retry as long as the length is unchanged...
            while (grown != null && Interlocked.CompareExchange(ref data, grown, prev) != prev & (prev = data)?.SlotCount == len);
            // && followed by & is VERY MUCH intended! (Logical followed by bitwise)
            return prev?.SlotCount > len;
        }

        // WARNING: does NOT check for duplicates!
        // WARNING: does NOT check that the user exists!
        internal bool TrySetSlot(int i, UserReady slot, Func<string, bool> allowOverwrite = null)
        {
            while (true)
            {
                var old = data;
                if (old == null)
                    return false;
                if (!(allowOverwrite ?? string.IsNullOrEmpty)(old[i].UserId))
                    return false;
                var @new = old.SetSlot(i, slot);
                if (Interlocked.CompareExchange(ref data, @new, old) == old)
                    return true;
            }
        }

        // also returns true if the user is already added
        // WARNING: does NOT check that the user exists!
        internal bool TryAddUser(string userId, out int slotIndex)
        {
            while (true)
            {
                var old = data;
                if (old == null)
                {
                    slotIndex = -1;
                    return false;
                }
                if (old.TryFindSlot(userId, out slotIndex) || old.TryFindOther(userId, out _))
                    return true;
                var @new = old.TryGetFirstEmptySlot(out slotIndex)
                    ? old.SetSlot(slotIndex, new UserReady(userId)) // <- never null
                    : old.TryAddToOthers(userId); // <- null on failure
                if (@new == null)
                    return false; // no empty slot and no room in others either!
                if (Interlocked.CompareExchange(ref data, @new, old) == old)
                    return true;
            }
        }

        // WARNING: does NOT check that the user exists!
        public bool TryLeaveLobby(string userId)
        {
            PhaseData old, @new;
            do @new = (old = data)?.LeaveLobby(userId);
            while (@new != null && Interlocked.CompareExchange(ref data, @new, old) != old);
            return old != null;
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
