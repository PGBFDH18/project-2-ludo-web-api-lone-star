using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CGetPlayerReady : IGetPlayerReady
    {
        private readonly ILudoService ludoService;

        public CGetPlayerReady(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public Error TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady)
        {
            var err = ludoService.GetPlayerReady(gameId, slot, out UserReady ur);
            playerReady = err == Error.Codes.E00NoError
                ? new PlayerReady { UserId = ur.UserId, Ready = ur.IsReady }
                : default;
            return err;
        }
    }

    public class CGetPlayerReadyMock : IGetPlayerReady
    {
        public Error TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady)
        {
            playerReady = new PlayerReady { UserId = "Placeholder", Ready = slot % 2 == 0 };
            return gameId == "test" ? Error.Codes.E01GameNotFound : Error.Codes.E00NoError;
        }
    }
}
