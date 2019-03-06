using System.Collections.Generic;

namespace Ludo.API.Service.Components
{
    public interface IListUsers
    {
        IEnumerable<string> ListUsers();
    }
}