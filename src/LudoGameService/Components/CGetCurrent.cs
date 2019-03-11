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
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err != Error.Codes.E00NoError)
                return err;
            turnSlotDie = ingame.GetCurrent();
            return Error.Codes.E00NoError;
        }
    }
}
