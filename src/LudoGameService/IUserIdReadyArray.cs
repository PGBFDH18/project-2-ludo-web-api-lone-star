using System.Collections.Generic;

namespace Ludo.GameService
{
    public interface IUserIdReadyArray : IUserIdArray
    {
        new UserReady this[int i] { get; }

        IReadOnlyList<string> Others { get; }

        bool IsAllReady { get; }
        bool IsEmpty { get; }
        //bool IsFull { get; }
    }
}
