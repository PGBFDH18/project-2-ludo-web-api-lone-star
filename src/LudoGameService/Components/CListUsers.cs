using System.Collections.Generic;
using System.Linq;

namespace Ludo.API.Service.Components
{
    public class CListUsers : IListUsers
    {
        private readonly ILudoService ludoService;

        public CListUsers(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public IEnumerable<string> ListUsers()
            => ludoService.Users.CreateIdSnapshot().Select(id => id.Encoded);
            //Placeholder:
            //=> new[] { "userId1", "userId2" };
    }
}
