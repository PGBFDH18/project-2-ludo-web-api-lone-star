using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetUser
    {
        UserInfo TryGetUser(string userId);
    }
}