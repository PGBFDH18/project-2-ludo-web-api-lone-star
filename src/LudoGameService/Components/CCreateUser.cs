namespace Ludo.API.Service.Components
{
    public class CCreateUser : ICreateUser
    {
        private readonly ILudoService ludoService;

        public CCreateUser(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public bool TryCreateUser(string userName, out string userId)
        {
            if (ludoService.Users.TryCreateUser(userName, out Id id))
            {
                userId = id.Encoded;
                return true;
            }
            userId = null;
            return false;
            //Placeholder:
            //userId = $"userId_placeholder({userName})";
            //return userName != "test";
        }
    }
}
