using System.Collections.Generic;

namespace Ludo.GameService
{
    public interface ISetupPhaseData : IUserIdArray
    {
        new UserReady this[int i] { get; }

        IReadOnlyList<string> Others { get; }
        IReadOnlyList<UserReady> Slots { get; }
        
        int SlotCount { get; }
        int PlayerCount { get; }
        int OtherCount { get; }

        bool IsAllReady { get; }
        new bool IsEmpty { get; }
        bool IsAlmostEmpty { get; }
        bool IsFull { get; }
        bool IsPenultimate { get; }

        // returns false iff slot is out of range.
        bool TryGet(int slot, out UserReady userReady);
    }
}
