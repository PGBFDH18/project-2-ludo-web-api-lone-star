using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface ISlotUser
    {
        Error TryClaimSlot(string gameId, int slot, string userId);
        Error TrySetSlotReady(string gameId, int slot, PlayerReady pr);
        Error TryUnSlotUser(string gameId, string userId);
    }
}