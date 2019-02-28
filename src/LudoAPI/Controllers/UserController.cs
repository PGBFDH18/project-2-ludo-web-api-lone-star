using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/user")]
    [ApiController]
    public class UserController : LudoControllerBase
    {
        private readonly Components.IListUsers listUser;
        private readonly Components.IFindUser findUser;
        private readonly Components.IUserNameAcceptable userNameAcceptable;
        private readonly Components.ICreateUser createUser;
        private readonly Components.IGetUser getUser;

        public UserController(
            Components.IListUsers listUser,
            Components.IFindUser findUser,
            Components.IUserNameAcceptable userNameAcceptable,
            Components.ICreateUser createUser,
            Components.IGetUser getUser)
        {
            this.listUser = listUser;
            this.findUser = findUser;
            this.userNameAcceptable = userNameAcceptable;
            this.createUser = createUser;
            this.getUser = getUser;
        }

        // operationId: ludoListUsers
        // 200 response: Done
        // 404 response: Done
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
        [ProducesResponseType(422, Type = typeof(ErrorCode))]
        [HttpPost] public IActionResult Post([FromHeader]string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest();
            var err = userNameAcceptable.IsUserNameAcceptable(userName);
            if (err != ErrorCodes.E00NoError)
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