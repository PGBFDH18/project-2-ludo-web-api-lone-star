using System;

namespace Ludo.GameService
{
    // TODO....
    public class LudoService : ILudoService
    {
        public UserStorage Users { get; } = new UserStorage();
        public GameStorage Games { get; } = new GameStorage();

        //public object Sessions

        public Error TryCreateLobby(string userId, int slots, out string gameId)
        {
            gameId = null;
            if (!Users.ContainsId(Id.Partial(userId)))
                return new Error(ErrorCodes.Err02UserNotFound);
            if (!GameLogic.SessionFactory.IsValid.PlayerCount(slots))
                return new Error(ErrorCodes.Err05InvalidSlotCount);
            var lobby = new SetupSession(userId, slots);
            var game = new GameData(lobby);
            gameId = Games.CreateGame(game).Encoded;
            return null;
        }
    }
}
