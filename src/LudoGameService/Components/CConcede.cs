using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CConcede : IConcede
    {
        private readonly ILudoService ludoService;

        public CConcede(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error Concede(string gameId, string userId) // TODO: auth!
            => ludoService.Concede(gameId, userId);
    }
}
