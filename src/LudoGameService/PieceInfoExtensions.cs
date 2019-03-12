namespace Ludo.API.Service.Extensions
{
    public static class PieceInfoExtensions
    {
        public static Models.PieceInfo AsModel(this GameLogic.PieceInfo pi)
            => new Models.PieceInfo
            {
                Collision = pi.Collision.AsModel(),
                Distance = pi.CurrentDistance,
                Moved = pi.MovedPosition,
                Position = pi.CurrentPosition,
            };

        public static Models.SlotPiece AsModel(this GameLogic.SlotPiece? sp)
            => sp == null
            ? null
            : new Models.SlotPiece
            {
                Piece = sp.GetValueOrDefault().Piece,
                Slot = sp.GetValueOrDefault().Slot,
            };
    }
}
