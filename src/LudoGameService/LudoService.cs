using System;
using System.Threading;

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
                return ErrorCodes.Err02UserNotFound;
            if (!GameLogic.SessionFactory.IsValid.PlayerCount(slots))
                return ErrorCodes.Err05InvalidSlotCount;
            var lobby = new SetupPhase(userId, slots);
            var game = new Game(lobby);
            gameId = Games.CreateGame(game).Encoded;
            return null;
        }

        public Error TryJoinLobby(string userId, string gameId, out int slot)
        {
            slot = -1;
            if (!Users.ContainsId(Id.Partial(userId)))
                return ErrorCodes.Err02UserNotFound;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return ErrorCodes.Err01GameNotFound;
            return game.TryAddUser(userId, out slot);
        }

        public Error TryLeaveLobby(string userId, string gameId)
        {
            if (!Users.ContainsId(Id.Partial(userId)))
                return ErrorCodes.Err02UserNotFound;
            var g = Games.TryGet(Id.Partial(userId));
            if (g == null)
                return ErrorCodes.Err01GameNotFound;
            if (g.Phase.Setup?.TryLeaveLobby(userId) != true)
                return ErrorCodes.Err03NotInSetupState;
            return null;
        }
    }
}
