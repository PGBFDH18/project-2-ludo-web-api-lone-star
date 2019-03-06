using Ludo.API.Models;
using Ludo.API.Service.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.API.Service.Components
{
    public class CListGames : IListGames
    {
        private readonly ILudoService ludoService;

        public CListGames(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public GameListEntry TryGetGame(string gameId)
            => (ludoService.Games.TryGet(Id.Partial(gameId))?.Phase)
            .IsNull(out IGamePhase game)
            ? null
            : new GameListEntry {
                GameId = gameId,
                Slots = game.Slots.ToArray(),
                State = game.Phase,
                Winner = game.Winner
            };

        public IEnumerable<GameListEntry> ListGames(ShowGame show)
        {
            switch (show)
            {
                default:
                case ShowGame.ingame:
                    return ludoService.Games
                        .Where(kvp => kvp.Value.Phase.Phase == GamePhase.ingame)
                        .Select(kvp => new GameListEntry {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = GamePhase.ingame,
                        });
                case ShowGame.finished:
                    return ludoService.Games
                        .Where(kvp => kvp.Value.Phase.Phase == GamePhase.finished)
                        .Select(kvp => new GameListEntry
                        {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = GamePhase.finished,
                            Winner = kvp.Value.Winner,
                        });
                case ShowGame.all:
                    return ludoService.Games
                        .Select(kvp => new GameListEntry
                        {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = kvp.Value.Phase.Phase,
                            Winner = kvp.Value.Winner,
                        });
            }
        }

        public IEnumerable<GameListEntry> TryListGames(ShowGame show, string userId)
        {
            if (!ludoService.Users.ContainsId(Id.Partial(userId)))
                return null;
            switch (show)
            {
                default:
                case ShowGame.ingame:
                    return ludoService.Games
                        .Where(kvp => kvp.Value.Phase.Phase == GamePhase.ingame
                            && kvp.Value.Phase.Slots.Contains(userId))
                        .Select(kvp => new GameListEntry
                        {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = Models.GamePhase.ingame,
                        });
                case ShowGame.finished:
                    return ludoService.Games
                        .Where(kvp => kvp.Value.Phase.Phase == GamePhase.finished
                            && kvp.Value.Phase.Slots.Contains(userId))
                        .Select(kvp => new GameListEntry
                        {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = GamePhase.finished,
                            Winner = kvp.Value.Winner,
                        });
                case ShowGame.all:
                    return ludoService.Games
                        .Where(kvp => kvp.Value.Phase.Slots.Contains(userId))
                        .Select(kvp => new GameListEntry
                        {
                            GameId = kvp.Key.Encoded,
                            Slots = kvp.Value.Phase.Slots.ToArray(),
                            State = kvp.Value.Phase.Phase,
                            Winner = kvp.Value.Winner,
                        });
            }
        }
    }
}
