using System.Collections.Generic;

namespace Ludo.API.Service
{
    public interface ISlotArray : IEnumerable<string>
    {
        string this[int i] { get; }
        // number of slots.
        int Length { get; }
        // number of non-empty slots.
        int PlayerCount { get; }
        // true if all slots are empty.
        bool IsEmpty { get; }
        // checks if index is in range.
        bool IsInRange(int index);
        // find index of user; otherwise -1.
        int IndexOf(string userId);
    }
}
