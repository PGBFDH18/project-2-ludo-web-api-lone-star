using System.Collections.Generic;

namespace Ludo.API.Service
{
    public interface ISetupPhaseData : ISlotArray
    {
        new UserReady this[int i] { get; }

        IReadOnlyList<string> Others { get; }
        IReadOnlyList<UserReady> Slots { get; }
        
        int SlotCount { get; }
        int OpenCount { get; }
        //int PlayerCount { get; } (inherited)
        int OtherCount { get; }

        // no further changes allowed (set when starting)
        bool IsFinalLocked { get; }
        bool IsAllReady { get; }
        new bool IsEmpty { get; }
        bool IsAlmostEmpty { get; }
        bool IsFull { get; }
        bool IsPenultimate { get; }

        // returns false iff slot is out of range.
        bool TryGet(int slot, out UserReady userReady);
    }
}
