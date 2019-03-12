using Ludo.GameLogic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ludo.GameLogic
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class TurnBegunEventArgs : EventArgs, IReadOnlyList<PieceInfo>
    {
        private readonly PieceInfo[] pieceInfos;

        public TurnBegunEventArgs(int turn, PieceInfo[] pieceInfos)
        {
            Turn = turn;
            this.pieceInfos = pieceInfos;
        }

        public int Turn { get; }

        public PieceInfo this[int i] => pieceInfos[i];
        
        int IReadOnlyCollection<PieceInfo>.Count => ((IReadOnlyList<PieceInfo>)pieceInfos).Count;
        
        public IEnumerator<PieceInfo> GetEnumerator()
            => ((IReadOnlyList<PieceInfo>)pieceInfos).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => pieceInfos.GetEnumerator();

        public PieceInfo[] Copy() => pieceInfos.Copy();
    }
}

