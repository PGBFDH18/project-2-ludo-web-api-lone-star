using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IGetUser
    {
        UserInfo TryGetUser(string userId);
    }
}