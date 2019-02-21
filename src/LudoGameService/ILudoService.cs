namespace Ludo.GameService
{
    public interface ILudoService
    {
        GameStorage Games { get; }
        UserStorage Users { get; }
    }
}