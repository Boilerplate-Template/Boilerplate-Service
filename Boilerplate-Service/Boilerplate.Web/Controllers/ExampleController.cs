using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// Example Controller
    /// </summary>
    [Route("example")]
    public class ExampleController : Controller
    {
        [HttpGet("")]
        public IActionResult DoStuff() { 
            /**/ 
            return View();
        }
    }
}
