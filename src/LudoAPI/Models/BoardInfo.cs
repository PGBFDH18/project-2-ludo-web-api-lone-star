using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ludo.WebAPI.Models
{
    public class BoardInfo
    {
        [BindRequired]
        public int BoardLength { get; set; }
        public int EndZoneLength { get; set; }
        public StartEndPos[] StartEndPositions { get; set; }

        public static implicit operator Models.BoardInfo(GameLogic.BoardInfo glbi)
            => new BoardInfo {
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
            }   };
    }
}
