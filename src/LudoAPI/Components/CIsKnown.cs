using Ludo.GameService;

namespace Ludo.WebAPI.Components
{
    sealed class CIsKnown : IIsKnown
    {
        private readonly ILudoService ludoService;
        public CIsKnown(ILudoService ludoService)
        {
            this.ludoService = ludoService;
        }

        public bool GameId(string gameId)
            => ludoService.Games.ContainsId(Id.Partial(gameId));

        public bool UserId(string userId)
            => ludoService.Users.ContainsId(Id.Partial(userId));
    }
}
