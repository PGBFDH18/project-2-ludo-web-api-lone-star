using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public interface IConcede
    {
        Error Concede(string gameId, string userId);
    }
}