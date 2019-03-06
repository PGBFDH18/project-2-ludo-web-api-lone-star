using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IGetCurrent
    {
        Error GetCurrent(string gameId, out TurnSlotDie turnSlotDie);
    }
}