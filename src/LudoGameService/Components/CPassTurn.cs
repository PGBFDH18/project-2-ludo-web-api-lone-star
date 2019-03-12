using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CPassTurn : IPassTurn
    {
        private readonly ILudoService ludoService;

        public CPassTurn(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error PassTurn(string gameId, int slot)
        {
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err)
                return err;
            if (!ingame.IsValidSlot(slot))
                return Error.Codes.E10InvalidSlotIndex;
            if (ingame.Slots[slot] == null)
                return Error.Codes.E16SlotIsEmpty;
            return ingame.TryPassTrun(slot)
                ? Error.Codes.E00NoError
                : Error.Codes.E15NotYourTurn;
        }
    }
}
