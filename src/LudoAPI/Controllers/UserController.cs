using Ludo.GameService;
using Ludo.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// Placeholder: Done
// Proper Code: TODO
namespace Ludo.WebAPI.Controllers
{
    [Route("ludo/user")]
    [ApiController]
    public class UserController : LudoControllerBase
    {
        private readonly ILudoService ludoService;
        private readonly Components.IIsKnown isKnown;

        public UserController(ILudoService ludoService, Components.IIsKnown isKnown)
        {
            this.ludoService = ludoService;
            this.isKnown = isKnown;
        }

        // operationId: ludoListUsers
        // 200 response: Done
        // 404 response: Done
        [HttpGet] public ActionResult<IEnumerable<string>> ListUsers([FromQuery]string userName)
        {
            IEnumerable<string> result;
            if (string.IsNullOrEmpty(userName))
                result = ListUsers();
            else if (!TryFindUser(userName, out result))
                return NotFound();
            return new ActionResult<IEnumerable<string>>(result);
            // implicit cast doesn't work: C# doesn't support cast operators on interfaces :(
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
            if (!IsUserNameAcceptable(userName, out ErrorCode whyNot))
                return UnprocessableEntity(whyNot);
            if (TryCreateUser(userName, out string userId))
                return Created(userId, null);
            return Conflict(); // userName acceptable but not creatable implies not unique.
        }

        // -------------------------------------------------------------------

        // operationId: ludoGetUser
        // 200 response: Done
        // 404 response: Done
        [HttpGet("{userId:required}")]
        public ActionResult<UserInfo> GetUser ([FromRoute]string userId)
        {
            if (TryGetUser(userId, out UserInfo user))
                return user;
            if (!isKnown.UserId(userId))
                return NotFound();
            return Status(500);
        }

        // ===================================================================

        //TODO: refactor out to a dependency injected component
        private IEnumerable<string> ListUsers()
        {
            //TODO
            return new[] { "userId1", "userId2" };
        }

        //TODO: refactor out to a dependency injected component
        private bool TryFindUser(string userName, out IEnumerable<string> match)
        {
            //TODO
            match = new[] { "userId1" };
            return userName != "test";
        }

        //TODO: refactor out to a dependency injected component
        private bool TryCreateUser(string userName, out string userId)
        {
            ////TODO
            //userId = $"userId_placeholder({userName})";
            //return userName != "test";
            if (ludoService.Users.TryCreateUser(userName, out Id id))
            {
                userId = id.Encoded;
                return true;
            }
            userId = null;
            return false;
        }

        //TODO: refactor out to a dependency injected component
        private bool TryGetUser(string userId, out UserInfo user)
        {
            //TODO
            user = new UserInfo { UserName = "xXx_Placeholder_xXx" };
            return userId != "test";
        }

        //TODO: refactor out to a dependency injected component
        private bool IsUserNameAcceptable(string userName, out ErrorCode whyNot)
        {
            whyNot = IsLengthError() ?? HasIllegalCharError() ?? IsReservedError() ?? IsProfaneError();
            return whyNot == null;

            ErrorCode IsLengthError()
            {
                const int MIN_LEN = 3;
                const uint MAX_LEN = 15;
                return unchecked((uint)(userName.Length - MIN_LEN) > (MAX_LEN - MIN_LEN))
                    ? new ErrorCode { Code = 1, Desc = $"Min length {MIN_LEN}; Max length {MAX_LEN}"}
                    : null;
            }

            ErrorCode HasIllegalCharError()
            {
                //TODO
                return userName.Contains(' ')
                    ? new ErrorCode { Code = 2, Desc = "Contains an illegal character." } //TODO: describe legal characters!
                    : null;
            }

            ErrorCode IsReservedError()
            {
                //TODO
                return userName == "test"
                    ? new ErrorCode { Code = 3, Desc = "That username is not allowed. Please pick another." }
                    : null;
            }

            ErrorCode IsProfaneError()
            {
                //TODO? Profanity filters are hopelessly messy... ignore for now.
                return userName.Contains("cunt")
                    ? new ErrorCode { Code = 3, Desc = "That username is not allowed. Please pick another." }
                    : null;
            }
        }
    }
}