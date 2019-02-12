namespace Ludo.WebAPI.Models
{
    public enum LobbyAccess
    {
        @public = 0, // default
        unlisted,
        friendsOnly,
        inviteOnly,
        reservations,
    }
}
