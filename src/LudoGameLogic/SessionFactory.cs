﻿using System;

namespace Ludo.GameLogic
{
    // http://www.playluggage.com/ludo-rules
    // ^så jävla många rule variants alltså...
    public static class SessionFactory
    {
        // startingPlayer == -1 means random starting player.
        public static ISession New(int playerCount = 2, Rules rules = default(Rules), int startingPlayer = -1, int boardLength = 40)
        {
            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentOutOfRangeException(nameof(playerCount));
            if (startingPlayer < -1 || startingPlayer >= playerCount)
                throw new ArgumentOutOfRangeException(nameof(startingPlayer));
            if (boardLength < 24 || boardLength > 100)
                throw new ArgumentOutOfRangeException(nameof(boardLength));
            if (boardLength % 8 != 0)
                throw new ArgumentException("Must be a multiple of 8.", nameof(boardLength));

            return new Session(playerCount, new BoardInfo(boardLength), rules, startingPlayer);
        }

        // Loads a gamestate.
        public static ISession Load(LudoSave save)
        {
            // TODO: kontrollera att spar-data är giltig
            return new Session(save);
        }
    }
}