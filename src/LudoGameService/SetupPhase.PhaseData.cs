using Ludo.API.Service.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.API.Service
{
    public partial class SetupPhase
    {
        // immutable!
        private class PhaseData : ISetupPhaseData
        {
            private readonly UserReady[] slots;
            private readonly string[] others;

            public PhaseData(UserReady[] slots, string[] others, bool isFinal = false)
            {
                this.slots = slots ?? throw new ArgumentNullException(nameof(slots));
                this.others = others ?? throw new ArgumentNullException(nameof(others));
                this.IsFinalLocked = isFinal;
            }

            // WARNING: This class does not check this property!
            // It is the responsibility of the SetupPhase class to check it!
            public bool IsFinalLocked { get; }

            public UserReady this[int i] => slots[i];

            string ISlotArray.this[int i] => slots[i].UserId;
            int ISlotArray.Length => slots.Length;
            bool ISlotArray.IsInRange(int index) => unchecked((uint)index < (uint)slots.Length);
            int ISlotArray.IndexOf(string userId) => Array.FindIndex(slots, (s) => s.UserId == userId);

            IReadOnlyList<string> ISetupPhaseData.Others => Array.AsReadOnly(others);
            IReadOnlyList<UserReady> ISetupPhaseData.Slots => Array.AsReadOnly(slots);

            // WARNING: OpenCount + PlayerCount != SlotCount
            public int SlotCount => slots.Length;
            public int OpenCount => IsFinalLocked ? 0 : slots.Count(s => !s.HasValue);
            public int PlayerCount => slots.Count(s => s.HasValue);
            public int OtherCount => others.Length;
            private int AllCount => OtherCount + PlayerCount;
            public bool IsAllReady => slots.All(s => s.IsReady || !s.HasValue);
            public bool IsEmpty => AllCount == 0;
            public bool IsAlmostEmpty => AllCount == 1;
            public bool IsFull => OpenCount == 0;
            public bool IsPenultimate => OpenCount == 1;

            public bool TryGet(int slot, out UserReady userReady)
            {
                if (unchecked((uint)slot < (uint)SlotCount))
                {
                    userReady = slots[slot];
                    return true;
                }
                userReady = default;
                return false;
            }

            internal PhaseData TryFinalLock()
                => IsAllReady ? new PhaseData(slots, others, true) : null;

            internal PhaseData TryAddSlot()
                => slots.Length < GameLogic.SessionFactory.MaxPlayers
                ? new PhaseData(slots.CopyResize(1), others)
                : null;

            // WARNING: does NOT check slot index range!
            // WARNING: does NOT check that user exists!
            internal PhaseData TrySetSlotReady(int slot, UserReady ur)
                => slots[slot].UserId == ur.UserId
                ? slots[slot].IsReady == ur.IsReady
                ? this // no change
                : SetSlot(slot, ur)
                : null; // userId mismatch

            // WARNING: does NOT check slot index range!
            // WARNING: does NOT check for duplicates!
            // WARNING: does NOT check MaxUsers!
            private PhaseData SetSlot(int slot, UserReady ur)
                => new PhaseData(slots.Copy().Modify(slot, ur), others);

            // WARNING: does NOT check that user exists!
            internal PhaseData TryAddUser(string userId, out int slot)
            {
                slot = -1;
                for (int i = SlotCount - 1; i >= 0; --i)
                {
                    string u = slots[i].UserId;
                    if (u == null)
                        slot = i;
                    else if (u == userId)
                        return this;
                }
                if (AllCount >= MaxUsers)
                    return null;
                if (slot == -1) // no vacant slots
                    return TryAddToOthers();
                else
                    return SetSlot(slot, new UserReady(userId));

                PhaseData TryAddToOthers()
                    => TryFindOther(userId, out _)
                    ? this
                    : new PhaseData(slots, others.CopyAppend(userId));
            }

            // returns null if the user isn't in the lobby (no change)
            // WARNING: does NOT check that user exists!
            internal PhaseData LeaveLobby(string userId)
            {
                // A user can not be in slots and others at the same time.
                if (TryFindSlot(userId, out int index))
                    return new PhaseData(slots.Copy().Modify(index, default), others);
                if (TryFindOther(userId, out index))
                    return new PhaseData(slots, others.CopyRemoveAt(index));
                return null;
            }

            // WARNING: does NOT check that user exists!
            public bool Contains(string userId)
                => TryFindSlot(userId, out _) || TryFindOther(userId, out _);

            // WARNING: does NOT check that user exists!
            public bool TryFindSlot(string userId, out int index)
                => (index = Array.FindIndex(slots, (s) => s.UserId == userId)) != -1;

            // WARNING: does NOT check that user exists!
            public bool TryFindOther(string userId, out int index)
                => (index = Array.IndexOf(others, userId)) != -1;

            // WARNING: does NOT check that user exists!
            internal PhaseData TryMoveFromSlotToOthers(string userId)
                => TryFindSlot(userId, out int i)
                ? new PhaseData(slots.Copy().Modify(i, default), others.CopyAppend(userId))
                : null;

            // WARNING: does NOT check slot index range!
            // WARNING: does NOT check that user exists!
            internal PhaseData TryMoveToSlot(int slot, string userId, bool allowEvict)
            {
                if (TryFindSlot(userId, out int index))
                {
                    if (slot == index)
                        return this;
                    if (allowEvict || !slots[index].HasValue)
                        return SwapSlots(index, slot);
                }
                else if (TryFindOther(userId, out index))
                {
                    if (allowEvict || !slots[index].HasValue)
                        return SwapSlotOther(slot, index);
                }
                return null;
            }

            // WARNING: does NOT check slot index range!
            private PhaseData SwapSlots(int slot1, int slot2)
                => slot1 == slot2
                ? this
                : new PhaseData(slots.Copy().Swap(slot1, slot2), others);

            // WARNING: does NOT check index ranges!
            private PhaseData SwapSlotOther(int slot, int other)
                => slots[slot].HasValue
                ? new PhaseData(slots.Copy().Modify(slot, new UserReady(others[other])), others.Copy().Modify(other, slots[slot].UserId))
                : new PhaseData(slots.Copy().Modify(slot, new UserReady(others[other])), others.CopyRemoveAt(other));

            // WARNING: does NOT check slot index range!
            // WARNING: does NOT check that slot is vacant!
            private PhaseData TryMoveFromOthersToSlot(int slot, UserReady ur)
                => TryFindOther(ur.UserId, out int otherIndex)
                ? new PhaseData(slots.Copy().Modify(slot, ur), others.CopyRemoveAt(otherIndex))
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
