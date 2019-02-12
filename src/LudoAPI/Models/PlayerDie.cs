using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ludo.WebAPI.Models
{
    public class PlayerDie
    {
        [BindRequired]
        public int Player { get; set; }
        [BindRequired]
        public int Die { get; set; }
    }
}
