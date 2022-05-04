using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// Values Controller 
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// <![CDATA[GET: api/<ValuesController>]]>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /Values
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// <![CDATA[GET api/<ValuesController>/5]]>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /Values/1
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// <![CDATA[POST api/<ValuesController>]]>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post /Values/1
        ///     { }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// <![CDATA[PUT api/<ValuesController>/5]]>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     Put /Values/1
        ///     { value: 'value' }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// <![CDATA[DELETE api/<ValuesController>/5]]>
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     Delete /Values/1
        ///     { }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ValidateAntiForgeryToken]
        public void Delete(int id)
        {
        }
    }
}
