using System;
using System.Threading;

namespace Ludo.GameService
{
    public partial class SetupSession : IGameStateSession, ISharedGSS
    {
        public static readonly int MaxOthers = 8;

        private volatile SetupState slots;

        // ASSUMES ownerId is a valid userId !!!
        public SetupSession(string ownerId, int slotCount)
        {
            GameLogic.SessionFactory.Validate.PlayerCount(slotCount);
            slots = new SetupState(new UserReady[slotCount].Modify(0, new UserReady(ownerId)), Array.Empty<string>());
        }

        public IUserIdReadyArray Data => slots;

        public bool TryAddSlot()
        {
            var prev = slots;
            var grown = prev;
            int len = prev.SlotCount;
            do grown = prev.TryAddSlot(); // retry as long as the length is unchanged...
            while (grown != null && Interlocked.CompareExchange(ref slots, grown, prev) != prev & (prev = slots).SlotCount == len);
            // && followed by & is VERY MUCH intended! (Logical followed by bitwise)
            return prev.SlotCount > len;
        }

        // WARNING: does NOT check for duplicates!
        internal bool TrySetSlot(int i, UserReady slot, Func<string, bool> allowOverwrite = null)
        {
            while (true)
            {
                var old = slots;
                if (!(allowOverwrite ?? string.IsNullOrEmpty)(old[i].UserId))
                    return false;
                var @new = old.SetSlot(i, slot);
                if (Interlocked.CompareExchange(ref slots, @new, old) == old)
                    return true;
            }
        }

        // also returns true if the user is already added
        public bool TryAddUser(string userId, out int slotIndex)
        {
            while (true)
            {
                var old = slots;
                if (old.TryFindSlot(userId, out slotIndex) || old.TryFindOther(userId, out _))
                    return true; // hmm...
                var @new = old.TryGetFirstEmptySlot(out slotIndex)
                    ? old.SetSlot(slotIndex, new UserReady(userId)) // <- never null
                    : old.TryAddToOthers(userId); // <- null on failure
                if (@new == null)
                    return false; // no empty slot and no room in others either!
                if (Interlocked.CompareExchange(ref slots, @new, old) == old)
                    return true;
            }
        }

        #region --- IGameStateSession ---
        GameState IGameStateSession.State => GameState.setup;

        SetupSession IGameStateSession.Setup => this;

        IngameSession IGameStateSession.Ingame => null;

        FinishedSession IGameStateSession.Finished => null;

        ISharedGSS IGameStateSession.Shared => this;

        IUserIdArray ISharedGSS.Slots => slots;
        #endregion
    }
}
