using Ludo.GameService;

namespace Ludo.WebAPI
{
    public abstract class LudoControllerBase : ExtendedControllerBase
    {
        protected internal const string ROUTE_gameId = "{gameId:required:minLength(2)}";
        protected internal const string ROUTE_slotStr = "{slotStr:required}";
        
        protected LudoControllerBase() { }
    }
}
