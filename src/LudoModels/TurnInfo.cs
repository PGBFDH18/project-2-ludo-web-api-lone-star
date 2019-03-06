namespace Ludo.API.Models
{
    public class TurnInfo
    {
        public bool CanPass { get; set; }
        public bool IsLucky { get; set; }
        public PieceInfo[] Pieces { get; set; }
    }
}
