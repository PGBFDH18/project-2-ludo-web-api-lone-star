namespace Ludo.API.Models
{
    public struct PlayerSlot
    {
        public readonly string Player;
        public readonly int Slot;

        public PlayerSlot(string id, int slot)
        {
            Player = id;
            Slot = slot;
        }
    }
}
