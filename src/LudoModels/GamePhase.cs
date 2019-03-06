namespace Ludo.API.Models
{
    // IMPORTANT:
    // Even values represent transitions.
    // Odd values represent more long lived states.
    public enum GamePhase
    {
        creating = 0,
        setup = 1,
        starting = 2,
        ingame = 3,
        ending = 4,
        finished = 5,
    }
    // NEVER CHANGE THESE VALUES!
    // (One should NEVER EVER change ANY public constant.)
}
