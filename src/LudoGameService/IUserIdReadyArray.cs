namespace Ludo.GameService
{
    public interface IUserIdReadyArray : IUserIdArray
    {
        new UserReady this[int i] { get; }
    }
}
