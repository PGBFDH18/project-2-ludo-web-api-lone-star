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
            return ingame.TryPassTrun(slot)
                ? Error.Codes.E00NoError
                : Error.Codes.E15NotYourTurn;
        }
    }
}
