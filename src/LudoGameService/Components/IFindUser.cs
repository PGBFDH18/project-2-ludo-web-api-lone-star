using System.Collections.Generic;

namespace Ludo.API.Service.Components
{
    public interface IFindUser
    {
        bool TryFindUser(string userName, out IEnumerable<string> match);
    }
}