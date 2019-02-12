namespace Ludo.WebAPI.Models
{
    public struct LobbySlot
    {
        public string Occupant { get; set; }
        public LobbySlotReservation Reserved { get; set; }
    }
}
