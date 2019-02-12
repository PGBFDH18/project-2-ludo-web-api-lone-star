namespace Ludo.WebAPI.Models
{
    public class PieceInfo
    {
        public int Distance { get; set; }
        public int Position { get; set; }
        public int Moved { get; set; }
        public PlayerPiece Collision { get; set; }
    }
}
