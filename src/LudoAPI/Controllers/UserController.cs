using Ludo.API.Models;
using Ludo.API.Service.Components;
using Ludo.API.Service.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo/user")]
    [ApiController]
    public class UserController : LudoControllerBase
    {
        private readonly IListUsers listUser;
        private readonly IFindUser findUser;
        private readonly IUserNameAcceptable userNameAcceptable;
        private readonly ICreateUser createUser;
        private readonly IGetUser getUser;

        public UserController(
            IListUsers listUser,
            IFindUser findUser,
            IUserNameAcceptable userNameAcceptable,
            ICreateUser createUser,
            IGetUser getUser)
        {
            this.listUser = listUser;
            this.findUser = findUser;
            this.userNameAcceptable = userNameAcceptable;
            this.createUser = createUser;
            this.getUser = getUser;
        }

        // operationId: ludoListUsers
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(404)]
        [HttpGet] public ActionResult<IEnumerable<string>> ListUsers([FromQuery]string userName)
        {
            IEnumerable<string> result;
            if (string.IsNullOrEmpty(userName))
                result = listUser.ListUsers();
            else if (!findUser.TryFindUser(userName, out result))
                return NotFound();
            return new ActionResult<IEnumerable<string>>(result);
        }

        // operationId: ludoCreateUser
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422, Type = typeof(Error))]
        [HttpPost] public IActionResult Post([FromHeader]string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest();
            var err = userNameAcceptable.IsUserNameAcceptable(userName);
            if (err)
                return UnprocessableEntity(err);
            if (createUser.TryCreateUser(userName, out string userId))
                return Created(userId, null);
            return Conflict(); // userName acceptable but not creatable implies not unique.
        }

        // -------------------------------------------------------------------

        // operationId: ludoGetUser
        [ProducesResponseType(200, Type = typeof(UserInfo))]
        [ProducesResponseType(404)]
        [HttpGet("{userId:required}")]
        public IActionResult GetUser ([FromRoute]string userId)
            => getUser.TryGetUser(userId).IsNull(out var user)
            ? (IActionResult)NotFound() : Ok(user);
    }
}