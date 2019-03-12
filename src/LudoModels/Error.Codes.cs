using System;

namespace Ludo.API.Models
{
    public readonly partial struct Error
    {
        public static class Codes
        {
            public static readonly int
                E00NoError = 0,
                E01GameNotFound = 1,
                E02UserNotFound = 2,
                E03NotInSetupPhase = 3,
                E04LobbyIsFull = 4,
                E05InvalidSlotCount = 5,
                E06BadUserName = 6,
                E07NotInGamePhase = 7,
                E08InvalidLobbyAccessValue = 8,
                E09CannotEvictSlotOccupant = 9,
                E10InvalidSlotIndex = 10,
                E11UserNotInLobby = 11,
                E12UserIdMismatch = 12,
                E13MaxSlotsReached = 13,
                E14NotAllUsersReady = 14,
                E15NotYourTurn = 15,
                E16SlotIsEmpty = 16,
                E17GameNotStarted = 17,
                E18InvalidPieceIndex = 18,
                E19GameRuleViolation = 19;
        }
    }
}
