using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommonControllerBase : ControllerBase
    {
        public CommonControllerBase()
        {
     
        }
    }
}
