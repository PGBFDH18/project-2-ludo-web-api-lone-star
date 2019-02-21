namespace Ludo.GameService
{
    public readonly struct UserReady
    {
        public UserReady(string userId, bool isReady = false)
        {
            UserId = userId;
            IsReady = isReady;
        }

        public string UserId { get; }
        public bool IsReady { get; }
    }
}
