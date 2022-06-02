using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// Common ControllerBase
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommonControllerBase : ControllerBase
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// AutoMapper
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// CommonControllerBase Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public CommonControllerBase(ILogger logger, IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
        }
    }
}
