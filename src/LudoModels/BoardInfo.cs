using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ludo.API.Models
{
    public class BoardInfo
    {
        [BindRequired]
        public int BoardLength { get; set; }
        public int EndZoneLength { get; set; }
        public StartEndPos[] StartEndPositions { get; set; }
    }
}
