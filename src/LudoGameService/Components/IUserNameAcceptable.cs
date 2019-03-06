using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IUserNameAcceptable
    {
        Error IsUserNameAcceptable(string userName);
    }
}