using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CGetCurrent : IGetCurrent
    {
        private readonly ILudoService ludoService;

        public CGetCurrent(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error GetCurrent(string gameId, out TurnSlotDie turnSlotDie)
        {
            turnSlotDie = null;
            var game = ludoService.Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var ingame = game.Phase.Ingame;
            if (ingame == null)
                return Error.Codes.E07NotInGamePhase;
            turnSlotDie = ingame.GetCurrent();
            return Error.Codes.E00NoError;
        }
    }
}
