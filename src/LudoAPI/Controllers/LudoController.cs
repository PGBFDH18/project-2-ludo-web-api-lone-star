using Ludo.GameService;
using Microsoft.AspNetCore.Mvc;

namespace Ludo.WebAPI.Controllers
{
    [Route("ludo")]
    [ApiController]
    public class LudoController : LudoControllerBase
    {
        public LudoController(ILudoService ludoService)
        { }

        [HttpGet]
        public void Get()
        {
            Response.StatusCode = 418; // I'm a teapot!
        }
    }
}