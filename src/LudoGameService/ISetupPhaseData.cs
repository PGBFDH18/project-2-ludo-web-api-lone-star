using System.Collections.Generic;

namespace Ludo.GameService
{
    public interface ISetupPhaseData : IUserIdArray
    {
        new UserReady this[int i] { get; }

        IReadOnlyList<string> Others { get; }
        IReadOnlyList<UserReady> Slots { get; }

        bool IsAllReady { get; }
    }
}
