using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface IGetPlayerReady
    {
        ErrorCode TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady);
    }
}