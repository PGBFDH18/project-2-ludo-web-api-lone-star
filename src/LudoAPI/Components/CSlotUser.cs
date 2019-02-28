using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CSlotUser : ISlotUser
    {
        private readonly ILudoService ludoService;

        public CSlotUser(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public ErrorCode TryClaimSlot(string gameId, int slot, string userId)
            => ludoService.ClaimSlot(gameId, slot, userId);

        public ErrorCode TrySetSlotReady(string gameId, int slot, PlayerReady pr)
            => ludoService.SetSlotReady(gameId, slot, new UserReady(pr.UserId, pr.Ready));

        public ErrorCode TryUnSlotUser(string gameId, string userId)
            => ludoService.UnSlotUser(gameId, userId);
    }
}
