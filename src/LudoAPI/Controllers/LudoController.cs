using Microsoft.AspNetCore.Mvc;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo")]
    [ApiController]
    public class LudoController : ControllerBase
    {
        [HttpGet]
        public void Get()
        {
            Response.StatusCode = 418; // I'm a teapot!
        }
    }
}