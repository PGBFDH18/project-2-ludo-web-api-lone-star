using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ludo.GameService
{
    public class SetupSession : IGameStateSession, ISharedGSS
    {
        public static readonly int MaxOthers = 8;

        private class SetupState : IUserIdReadyArray
        {
            private readonly UserReady[] slots;
            private readonly string[] others;

            public SetupState(UserReady[] slots, string[] others)
            {
                this.slots = slots ?? throw new ArgumentNullException(nameof(slots));
                this.others = others ?? throw new ArgumentNullException(nameof(others));
            }

            public UserReady this[int i] => slots[i];
            public int SlotCount => slots.Length;

            string IUserIdArray.this[int i] => slots[i].UserId;
            int IUserIdArray.Length => slots.Length;

            public SetupState TryAddSlot()
                => slots.Length < GameLogic.SessionFactory.MaxPlayers
                ? new SetupState(slots.CopyResize(1), others)
                : null;

            public SetupState TryAddToOthers(string userId)
                => others.Length < MaxOthers && !Contains(userId)
                ? new SetupState(slots, others.CopyAppend(userId))
                : null;

            // WARNING: does NOT check for duplicates!
            public SetupState SetSlot(int i, UserReady slot)
                => new SetupState(slots.Copy().Modify(i, slot), others);

            public SetupState LeaveLobby(string userId)
            {
                // A user can not be in slots and others at the same time.
                if (TryFindSlot(userId, out int i))
                    return new SetupState(slots.Copy().Modify(i, default), others);
                if (TryFindOther(userId, out i))
                    return new SetupState(slots, others.CopyRemoveAt(i));
                return this;
            }

            public bool Contains(string userId)
                => TryFindSlot(userId, out _) || TryFindOther(userId, out _);

            public bool TryFindSlot(string userId, out int index)
                => (index = Array.FindIndex(slots, (s) => s.UserId == userId)) != -1;

            public bool TryFindOther(string userId, out int index)
                => (index = Array.IndexOf(others, userId)) != -1;

            public SetupState TryMoveFromSlotToOthers(string userId)
                => others.Length < MaxOthers && TryFindSlot(userId, out int i)
                ? new SetupState(slots.Copy().Modify(i, default), others.CopyAppend(userId))
                : null;

            public SetupState TryMoveFromOthersToSlot(int i, UserReady slot)
                => TryFindOther(slot.UserId, out int i_o)
                ? new SetupState(slots.Copy().Modify(i, slot), others.CopyRemoveAt(i_o))
                : null;

            // iEmpty is -1 if false is returned.
            public bool TryGetFirstEmptySlot(out int iEmpty)
                => TryFindSlot(null, out iEmpty);

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                foreach (var v in slots)
                    yield return v.UserId;
            }

            IEnumerator IEnumerable.GetEnumerator()
                => ((IEnumerable<string>)this).GetEnumerator();
        }

        private volatile SetupState slots;

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
