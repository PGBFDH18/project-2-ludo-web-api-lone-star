using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.GameService
{
    public partial class SetupSession
    {
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

            IReadOnlyList<string> IUserIdReadyArray.Others => Array.AsReadOnly(others);
            IReadOnlyList<UserReady> IUserIdReadyArray.Slots => Array.AsReadOnly(slots);

            public int OpenCount => slots.Count((s) => string.IsNullOrEmpty(s.UserId));
            public bool IsAllReady => slots.All((s) => s.IsReady);
            public bool IsEmpty => others.Length == 0 && slots.All(((s) => string.IsNullOrEmpty(s.UserId)));
            //public bool IsFull => OpenCount == 0;

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
    }
}
