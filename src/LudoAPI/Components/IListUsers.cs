using System.Collections.Generic;

namespace Ludo.WebAPI.Components
{
    public interface IListUsers
    {
        IEnumerable<string> ListUsers();
    }
}