using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public interface ISlotUser
    {
        ErrorCode TryClaimSlot(string gameId, int slot, string userId);
        ErrorCode TrySetSlotReady(string gameId, int slot, PlayerReady pr);
        ErrorCode TryUnSlotUser(string gameId, string userId);
    }
}