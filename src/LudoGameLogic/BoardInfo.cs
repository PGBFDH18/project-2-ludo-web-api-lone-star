using System;

/* See board positions at the bottom! */

namespace Ludo.GameLogic
{
    public struct BoardInfo
    {
        public const int DEFAULT_LENGTH = 40;
        public static readonly int MinLength = 24;
        public static readonly int MaxLength = 80;
        private const int SLOT_COUNT = 4;

        public static class IsValid
        {
            public static bool Length(int boardLength)
                => boardLength >= MinLength & boardLength <= MaxLength & (boardLength % ENDZONE_DIVISOR == 0);
        }
        public static class Validate
        {
            public static void Length(int boardLength)
            {
                if (boardLength < MinLength | boardLength > MaxLength) throw new ArgumentOutOfRangeException(nameof(boardLength));
                if (boardLength % ENDZONE_DIVISOR != 0) throw new ArgumentException("Must be a multiple of 8.", nameof(boardLength));
            }
        }

        // ctor - does NOT validate the length argument!
        public BoardInfo(int boardLength = DEFAULT_LENGTH) // boardLength must be a multiple of 8.
            => Length = boardLength;

        // length of the shared track around the board (excluding the collision-free end-zones leading to the goal).
        public int Length { get; }

        // Number of player slots supported by the ludo board.
        public int SlotCount => SLOT_COUNT;

        // is this instance valid? (Was it initialized with a valid length argument?)
        public bool IsProper
            => IsValid.Length(Length);

        // width and height of a board (Size x Size) (helper method useful for drawing a GUI)
        [Obsolete] // TODO: move somewhere else - feels dirty to have a GUI related method here.
        public int Size
            => Length / 4 + 1; // Assumes standard shaped board with BASE_COUNT == 4, Size property is not well defined otherwise.

        // length of the collision-free end-zones leading to the goal (goal square inclusive).
        public int EndZoneLength
            => Length / ENDZONE_DIVISOR;

        // length of the board + length of end-zone.
        public int GoalDistance
            => Length + EndZoneLength;

        // where on the board does player X start (when they move a piece of from their base).
        public int StartPosition(int player)
            => (Length / BASE_COUNT) * player;

        // what board position corresponds to the first end-zone position of player X.
        public int EndZonePosition(int player)
            => Length + player * EndZoneLength;

        // checks if the distance of a piece corresponds to one of the collision-free end-zones (goal square exclusive).
        public bool IsInEndZone(int distance)
            => distance > Length && distance < GoalDistance;

        // checks if a position corresponds to a board square.
        public bool IsValidPosition(int position)
            => position >= 0 && position < GoalPosition(3)
            && position != GoalPosition(0) 
            && position != GoalPosition(1)
            && position != GoalPosition(2);

        // these positions are reserved. Currently not used they are not considered valid positions! (Base and Goal have position == -1)
        internal int GoalPosition(int player)
            => Length + (player + 1) * EndZoneLength - 1;

        private const int BASE_COUNT = 4;
        private const int ENDZONE_DIVISOR = 8;
    }
}
/*
     * Standard Ludo Board:
     * --------------------
     * Length: 40
     * Size: 11x11
     * EndZoneLength: 5 (4 + Goal)
     * Positions:

▶▶▶▶                        08 ▶ 09 ▶ 10  ⇠ (P1)
                            ▲    ↓    ▼
▶▶▶▶                        07   45   11
                            ▲    ↓    ▼
▶▶▶▶                        06   46   12
                            ▲    ↓    ▼
▶▶▶▶    (P0)                05   47   13
        ⇣                   ▲    ↓    ▼
        00 ▶ 01 ▶ 02 ▶ 03 ▶ 04   48   14 ▶ 15 ▶ 16 ▶ 17 ▶ 18
        ▲                        ↓                        ▼
        39 → 40 → 41 → 42 → 43 → G ← 53 ← 52 ← 51 ← 50 ← 19
        ▲                        ↑                        ▼
        38 ◀ 37 ◀ 36 ◀ 35 ◀ 34   58   24 ◀ 23 ◀ 22 ◀ 21 ◀ 20
                            ▲    ↑    ▼                   ⇡
▶▶▶▶                        33   57   25                (P2)
                            ▲    ↑    ▼
▶▶▶▶                        32   56   26
                            ▲    ↑    ▼
▶▶▶▶                        31   55   27
                            ▲    ↑    ▼
▶▶▶▶                (P3) ⇢  30 ◀ 29 ◀ 28

    Remember that for P0, position 0 corresponds to distance 1,
    since distance 0 corresponds to the base. Care not to mix them up!
 */
