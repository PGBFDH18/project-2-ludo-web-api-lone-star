using System.Collections.Generic;

namespace Ludo.GameService
{
    public interface ISlotArray : IEnumerable<string>
    {
        string this[int i] { get; }
        int Length { get; }

        int PlayerCount { get; }
        bool IsEmpty { get; }
    }
}
