using Ludo.API.Models;

namespace Ludo.API.Service
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

        public bool HasValue => UserId != null;

        public static PlayerReady ToModel(UserReady ur) // static for elegant use with Linq
            => new PlayerReady { UserId = ur.UserId, Ready = ur.IsReady };
    }
}
