using System.Collections.Generic;

namespace Ludo.GameService
{
    public interface IUserIdArray : IEnumerable<string>
    {
        string this[int i] { get; }
        int Length { get; }

        int OpenCount { get; }
    }
}
