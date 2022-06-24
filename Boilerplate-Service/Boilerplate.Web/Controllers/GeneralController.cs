using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// General Controller
    /// </summary>
    public class GeneralController : CommonControllerBase
    {
        /// <summary>
        /// GeneralController Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public GeneralController(ILogger<TodoItemsController> logger, IMapper mapper)
            : base(logger, mapper)
        {
            
        }

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
        /// 파일 가져오기
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns></returns>
        [HttpGet("{fileName}")]
        [Produces("application/octet-stream", Type = typeof(VirtualFileResult))]
        public VirtualFileResult GetFile(string fileName)
        {
            // 임시폴더에서 파일 찾기
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("파일이 없습니다.", fileName: fileName);
            }

            return File(virtualPath: fileName, contentType: "application/octet-stream", fileDownloadName: fileName);
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
