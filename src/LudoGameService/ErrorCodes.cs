using System;

namespace Ludo.GameService
{
    public static class ErrorCodes
    {
        public static readonly int Err0None = 0;
        public static readonly int Err1GameNotFound = 1;
        public static readonly int Err2UserNotFound = 2;
        public static readonly int Err3NotInSetupState = 3;
        public static readonly int Err4LobbyIsFull = 4;
    }
}
