using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetPlayerReady
    {
        Error TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady);
    }
}