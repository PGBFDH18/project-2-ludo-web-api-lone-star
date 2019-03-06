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
            var g = ludoService.Games.TryGet(Id.Partial(gameId));
            if (g == null)
                return Error.Codes.E01GameNotFound;
            var ingame = g.Phase.Ingame;
            if (ingame == null)
                return Error.Codes.E07NotInGamePhase;
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
