using Ludo.GameLogic.Extensions;
using System;

namespace Ludo.GameLogic
{
    public struct LudoSave
    {
        public int TurnCounter { get; }
        public int CurrentSlot { get; }
        public int CurrentDieRoll { get; }
        public int BoardLength { get; }
        public Rules Rules { get; }

        public int PlayerCount 
            => pieceDistances.GetLength(0);

        public int GetPieceDistance(int slot, int piece)
            => pieceDistances[slot]?[piece] ?? 0;

        internal readonly int[][] pieceDistances;

        // ctor
        public LudoSave(int turn, int slot, int dieRoll, int boardLength, Rules rules, int[][] playerPieceDistances)
        {
            TurnCounter = turn;
            CurrentSlot = slot;
            CurrentDieRoll = dieRoll;
            BoardLength = boardLength;
            pieceDistances = playerPieceDistances.JaggedCopy();
            Rules = rules;
        }
    }
}
