using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ludo.API.Models
{
    public class TurnSlotDie
    {
        [BindRequired]
        public int Turn { get; set; }
        [BindRequired]
        public int Slot { get; set; }
        [BindRequired]
        public int Die { get; set; }

        public static explicit operator TurnSlotDie((int,int,int) tsd)
            => new TurnSlotDie { Turn = tsd.Item1, Slot = tsd.Item2, Die = tsd.Item3 };
    }
}
