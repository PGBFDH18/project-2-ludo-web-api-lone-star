using Ludo.API.Service.Extensions;
using Ludo.GameLogic;
using System.Linq;

namespace Ludo.API.Service
{
    public partial class IngamePhase
    {
        private class TurnCache
        {
            public readonly int Turn;
            public readonly int Slot;
            public readonly Models.PieceInfo[] pieces;

            public TurnCache(TurnBegunEventArgs tbe, int currentSlot)
            {
                Turn = tbe.Turn;
                Slot = currentSlot;
                pieces = tbe.Select(PieceInfoExtensions.AsModel).ToArray();
            }
        }
    }
}
