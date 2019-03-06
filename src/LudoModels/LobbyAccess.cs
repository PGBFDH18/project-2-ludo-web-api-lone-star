namespace Ludo.API.Models
{
    public enum LobbyAccess
    {
        @public = 0, // default
        unlisted,
        friendsOnly,
        inviteOnly,
        reservations,
    }
    // NEVER CHANGE THESE VALUES!
    // (One should NEVER EVER change ANY public constant.)
}
