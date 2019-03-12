using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CGetPieceInfo : IGetPieceInfo
    {
        private readonly ILudoService ludoService;

        public CGetPieceInfo(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        // TODO: rely on turn instead of slot!
        public Error GetPieceInfo(string gameId, int slot, int piece, out PieceInfo pieceInfo)
        {
            pieceInfo = null;
            var err = ludoService.GetIngame(gameId, out var ingame);
            if (err)
                return err;
            var ts = ingame.GetPieceInfo(piece, out pieceInfo);
            if (ts.slot == slot) // TODO: rely on turn instead of slot!
                return Error.Codes.E00NoError; // <- good path
            if (!ingame.IsValidSlot(slot))
                return Error.Codes.E10InvalidSlotIndex;
            if (ingame.Slots[slot] == null)
                return Error.Codes.E16SlotIsEmpty;
            if (ts.turn == 0)
                return Error.Codes.E17GameNotStarted;
            if (ts.slot == -1)
                return Error.Codes.E18InvalidPieceIndex;
            // slot is valid but not what we expected:
            return Error.Codes.E15NotYourTurn;
        }
    }
}
