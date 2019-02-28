using Ludo.GameService;
using Ludo.WebAPI.Models;

namespace Ludo.WebAPI.Components
{
    public class CGetPlayerReady : IGetPlayerReady
    {
        private readonly ILudoService ludoService;

        public CGetPlayerReady(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public ErrorCode TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady)
        {
            var err = ludoService.GetPlayerReady(gameId, slot, out UserReady ur);
            playerReady = err == ErrorCodes.E00NoError
                ? new PlayerReady { UserId = ur.UserId, Ready = ur.IsReady }
                : default;
            return err;
        }
    }

    public class CGetPlayerReadyMock : IGetPlayerReady
    {
        public ErrorCode TryGetPlayerReady(string gameId, int slot, out PlayerReady playerReady)
        {
            playerReady = new PlayerReady { UserId = "Placeholder", Ready = slot % 2 == 0 };
            return gameId == "test" ? ErrorCodes.E01GameNotFound : ErrorCodes.E00NoError;
        }
    }
}
