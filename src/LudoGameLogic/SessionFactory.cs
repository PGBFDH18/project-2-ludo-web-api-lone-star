using System;
using System.Runtime.CompilerServices;

namespace Ludo.GameLogic
{
    // http://www.playluggage.com/ludo-rules
    // ^så jävla många rule variants alltså...
    public static class SessionFactory
    {
        //public const int RANDOM_STARTING_PLAYER = -1;
        public static readonly int MinPlayers = 2;
        public static readonly int MaxPlayers = 4;

        public static class IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool PlayerCount(int playerCount)
                => MinPlayers <= playerCount & playerCount <= MaxPlayers;
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //public static bool StartingPlayer(int startingPlayer, int playerCount)
            //    => startingPlayer == RANDOM_STARTING_PLAYER | unchecked((uint)startingPlayer < (uint)playerCount);
        }

        public static class Validate
        {
            public static void PlayerCount(int playerCount)
            {
                if (!IsValid.PlayerCount(playerCount))
                    throw new ArgumentOutOfRangeException(nameof(playerCount), $"Min: {MinPlayers}, Max: {MaxPlayers}.");
            }
            //public static void StartingPlayer(int startingPlayer, int playerCount)
            //{
            //    if (!IsValid.StartingPlayer(startingPlayer, playerCount))
            //        throw new ArgumentOutOfRangeException(nameof(startingPlayer));
            //}
        }


        public static ISession New(Rules rules = default(Rules), int boardLength = BoardInfo.DEFAULT_LENGTH)
        {
            //Validate.PlayerCount(playerCount);
            //Validate.StartingPlayer(startingPlayer, playerCount);
            BoardInfo.Validate.Length(boardLength);

            return new Session(new BoardInfo(boardLength), rules);
        }

        // Loads a gamestate.
        public static ISession Load(LudoSave save)
        {
            // TODO: kontrollera att spar-data är giltig
            return new Session(save);
        }
    }
}
