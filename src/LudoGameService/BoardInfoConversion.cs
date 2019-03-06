using Ludo.API.Models;

namespace Ludo.API.Service.Extensions
{
    public static class BoardInfoConversion
    {
        public static BoardInfo ToModel(this GameLogic.BoardInfo glbi)
            => new BoardInfo
            {
                BoardLength = glbi.Length,
                EndZoneLength = glbi.EndZoneLength,
                StartEndPositions = new StartEndPos[]
                {
                    new StartEndPos {
                        StartPos = glbi.StartPosition(0),
                        EndZonePos = glbi.EndZonePosition(0),
                    },
                    new StartEndPos {
                        StartPos = glbi.StartPosition(1),
                        EndZonePos = glbi.EndZonePosition(1),
                    },
                    new StartEndPos {
                        StartPos = glbi.StartPosition(2),
                        EndZonePos = glbi.EndZonePosition(2),
                    },
                    new StartEndPos {
                        StartPos = glbi.StartPosition(3),
                        EndZonePos = glbi.EndZonePosition(3),
                    },
            }
            };
    }
}
