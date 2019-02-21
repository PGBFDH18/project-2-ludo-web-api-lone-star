using System;

namespace Ludo.GameService
{
    public static class ErrorCodes
    {
        public static readonly int Err00None = 0;
        public static readonly int Err01GameNotFound = 1;
        public static readonly int Err02UserNotFound = 2;
        public static readonly int Err03NotInSetupState = 3;
        public static readonly int Err04LobbyIsFull = 4;
        public static readonly int Err05InvalidSlotCount = 5;
    }
}
