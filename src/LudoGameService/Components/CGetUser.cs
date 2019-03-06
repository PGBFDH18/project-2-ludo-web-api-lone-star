using Ludo.API.Models;

namespace Ludo.API.Service.Components
{
    public class CGetUser : IGetUser
    {
        private readonly ILudoService ludoService;

        public CGetUser(ILudoService ludoService) {
            this.ludoService = ludoService;
        }

        public UserInfo TryGetUser(string userId)
            => ludoService.Users.TryGetUserName(Id.Partial(userId), out string userName)
            ? new UserInfo { UserName = userName }
            : null;
            //Placeholder:
            //=> userId == "test" ? null : new UserInfo { UserName = "xXx_Placeholder_xXx" };
    }
}
