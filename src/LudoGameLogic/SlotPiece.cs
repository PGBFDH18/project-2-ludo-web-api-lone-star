using System;

namespace Ludo.GameLogic
{
    public struct SlotPiece
    {
        // who does the colliding piece belong to?
        public int Slot { get; }

        // which of the target players pieces is it?
        public int Piece { get; }

        // ctor
        public SlotPiece(int slot, int piece)
        {
            Slot = slot;
            Piece = piece;
        }
    }
}
