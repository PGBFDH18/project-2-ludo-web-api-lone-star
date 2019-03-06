namespace Ludo.API.Service.Components
{
    public interface ICreateUser
    {
        bool TryCreateUser(string userName, out string userId);
    }
}