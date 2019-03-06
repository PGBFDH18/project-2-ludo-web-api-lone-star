using Ludo.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.API.Web.Controllers
{
    [Route("ludo")]
    [ApiController]
    public class LudoController : LudoControllerBase
    {
        public LudoController(ILudoService ludoService)
        { }

        [HttpGet] public void Get()
        {
            // TODO: useful response...
            Response.StatusCode = 418; // I'm a teapot!
        }
    }
}