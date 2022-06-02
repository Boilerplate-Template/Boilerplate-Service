using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// General Controller
    /// </summary>
    public class GeneralController : CommonControllerBase
    {
        /// <summary>
        /// Fire Exception
        /// </summary>
        /// <param name="isFireException"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("FireException")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FireException(bool isFireException)
        {
            if (isFireException)
            {
                //return await Task.FromResult();
                throw new Exception("강제 에러 발생");
            }
            else
            {
                //throw new Exception("This is an exception thrown from middleware.");
                return await Task.FromResult(NotFound());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/error-development")]
        public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/error")]
        public IActionResult HandleError() =>
            Problem();
    }
}
