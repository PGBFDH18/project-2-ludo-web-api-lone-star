using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IUserNameAcceptable
    {
        ErrorCode IsUserNameAcceptable(string userName);
    }
}