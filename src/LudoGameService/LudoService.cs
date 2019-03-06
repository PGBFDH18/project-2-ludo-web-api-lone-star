﻿using Ludo.API.Models;
using Ludo.API.Service.Extensions;
using System;

namespace Ludo.API.Service
{
    internal class LudoService : ILudoService
    {
        public UserStorage Users { get; } = new UserStorage();
        public GameStorage Games { get; } = new GameStorage();

        public Error CreateLobby(string userId, int slots, LobbyAccess access, out string gameId)
        {
            gameId = null;
            if (!access.IsDefined())
                return Error.Codes.E08InvalidLobbyAccessValue;
            if (!Users.ContainsId(Id.Partial(userId)))
                return (Error.Codes.E02UserNotFound, userId);
            if (!GameLogic.SessionFactory.IsValid.PlayerCount(slots))
                return Error.Codes.E05InvalidSlotCount;
            var lobby = new SetupPhase(userId, slots, access);
            var game = new Game(lobby);
            gameId = Games.CreateGame(game).Encoded;
            return Error.Codes.E00NoError;
        }

        public Error GetPlayerReady(string gameId, int slot, out UserReady userReady)
        {
            userReady = default;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var sed = game.Phase.Setup.Data;
            if (sed == null)
                return Error.Codes.E03NotInSetupPhase;
            return sed.TryGet(slot, out userReady)
                ? Error.Codes.E00NoError
                : Error.Codes.E10InvalidSlotIndex;
        }

        public Error JoinLobby(string userId, string gameId, out int slot)
        {
            slot = -1;
            if (!Users.ContainsId(Id.Partial(userId)))
                return Error.Codes.E02UserNotFound;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            return game.TryAddUser(userId, out slot);
        }

        public Error LeaveLobby(string userId, string gameId)
        {
            if (!Users.ContainsId(Id.Partial(userId)))
                return Error.Codes.E02UserNotFound;
            var game = Games.TryGet(Id.Partial(userId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var setup = game.Phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            bool wasLastUser = false;
            var err = setup.TryLeaveLobby(userId, out wasLastUser);
            if (wasLastUser) // last user has left the lobby?
                Games.Remove(Id.Partial(userId), out _);
            return err;
        }
        
        public Error SetSlotReady(string gameId, int slot, UserReady userReady)
        {
            if (!Users.ContainsId(Id.Partial(userReady.UserId)))
                return Error.Codes.E02UserNotFound;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var setup = game.Phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            return setup.TrySetSlotReady(slot, userReady);
        }
        
        public Error UnSlotUser(string gameId, string userId)
        {
            if (!Users.ContainsId(Id.Partial(userId)))
                return Error.Codes.E02UserNotFound;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var setup = game.Phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            return setup.TryUnSlot(userId);
        }

        public Error ClaimSlot(string gameId, int slot, string userId)
        {
            if (!Users.ContainsId(Id.Partial(userId)))
                return Error.Codes.E02UserNotFound;
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            var setup = game.Phase.Setup;
            if (setup == null)
                return Error.Codes.E03NotInSetupPhase;
            return setup.TryClaimSlot(slot, userId, null); // <--- TODO: reservations!
        }

        public Error StartGame(string gameId, int startingSlot = -1)
        {
            var game = Games.TryGet(Id.Partial(gameId));
            if (game == null)
                return Error.Codes.E01GameNotFound;
            return game.TryStartGame(startingSlot);
            // TODO: loading a saved game
        }
    }
}
