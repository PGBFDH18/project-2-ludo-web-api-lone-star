namespace Ludo.WebAPI.Components
{
    public interface ICreateUser
    {
        bool TryCreateUser(string userName, out string userId);
    }
}