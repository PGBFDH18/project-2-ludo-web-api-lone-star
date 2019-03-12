using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CGetTurnInfo : IGetTurnInfo
    {
        private readonly ILudoService ludoService;

        public CGetTurnInfo(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error GetTurnInfo(string gameId, int slot, out TurnInfo turnInfo)
        {
            turnInfo = null;
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err)
                return err;
            if (!ingame.IsValidSlot(slot))
                return Error.Codes.E10InvalidSlotIndex;
            if (ingame.Slots[slot] == null)
                return Error.Codes.E16SlotIsEmpty;
            turnInfo = ingame.TryGetTurnInfo(slot);
            return turnInfo == null
                ? Error.Codes.E15NotYourTurn
                : Error.Codes.E00NoError;
        }
    }
}
