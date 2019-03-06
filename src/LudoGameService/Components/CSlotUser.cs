using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CSlotUser : ISlotUser
    {
        private readonly ILudoService ludoService;

        public CSlotUser(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryClaimSlot(string gameId, int slot, string userId)
            => ludoService.ClaimSlot(gameId, slot, userId);

        public Error TrySetSlotReady(string gameId, int slot, PlayerReady pr)
            => ludoService.SetSlotReady(gameId, slot, new UserReady(pr.UserId, pr.Ready));

        public Error TryUnSlotUser(string gameId, string userId)
            => ludoService.UnSlotUser(gameId, userId);
    }
}
