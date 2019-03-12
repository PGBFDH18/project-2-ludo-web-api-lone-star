using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CMovePiece : IMovePiece
    {
        private readonly ILudoService ludoService;

        public CMovePiece(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error MovePiece(string gameId, int slot, int piece)
        {
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err)
                return err;
            return ingame.MovePiece(slot, piece);
        }
    }
}
