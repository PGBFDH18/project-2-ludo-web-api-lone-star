using Ludo.API.Models;

namespace Ludo.API.Service
{
    public static class ServiceExtensions
    {
        public static Error GetIngame(this ILudoService service, string gameId, out IngamePhase ingame)
        {
            ingame = null;
            var g = service.Games.TryGet(Id.Partial(gameId));
            if (g == null)
                return Error.Codes.E01GameNotFound;
            ingame = g.Phase.Ingame;
            return ingame == null
                ? Error.Codes.E07NotInGamePhase
                : Error.Codes.E00NoError;
        }
    }
}
