namespace Ludo.API.Models
{
    public class BoardState
    {
        public BoardState() {}
        public BoardState(int[][] playerPieces)
            => PlayerPieces = playerPieces;

        // [slotIndex][pieceIndex]
        public int[][] PlayerPieces { get; set; } 
    }
}
