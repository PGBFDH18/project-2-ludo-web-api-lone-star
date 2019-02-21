using Ludo.GameService;

namespace Ludo.WebAPI.Components
{
    sealed class IsKnown : IIsKnown
    {
        private readonly ILudoService ludoService;
        public IsKnown(ILudoService ludoService)
        {
            this.ludoService = ludoService;
        }

        public bool GameId(string gameId)
            => ludoService.Games.ContainsId(Id.Partial(gameId));

        public bool UserId(string userId)
            => ludoService.Users.ContainsId(Id.Partial(userId));
    }
}
