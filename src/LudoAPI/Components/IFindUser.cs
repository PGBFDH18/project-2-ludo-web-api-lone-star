using System.Collections.Generic;

namespace Ludo.WebAPI.Components
{
    public interface IFindUser
    {
        bool TryFindUser(string userName, out IEnumerable<string> match);
    }
}