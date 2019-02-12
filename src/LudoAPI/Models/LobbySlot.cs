namespace Ludo.WebAPI.Models
{
    public class LobbySlot
    {
        public string Occupant { get; set; }
        public LobbySlotReservation Reserved { get; set; }
    }
}
