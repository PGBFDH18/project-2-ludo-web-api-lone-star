using System.Collections.Generic;

namespace Ludo.API.Service.Components
{
    public class CFindUser : IFindUser
    {
        private readonly ILudoService ludoService;

        public CFindUser(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public bool TryFindUser(string userName, out IEnumerable<string> match)
        {
            if (ludoService.Users.TryGetId(userName, out Id id))
            {
                match = new[] { id.Encoded };
                return true;
            }
            match = null;
            return false;
            //Placeholder:
            //match = new[] { "userId1" };
            //return userName != "test";
        }
    }
}
