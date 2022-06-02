using Boilerplate.Web.Context;
using Boilerplate.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;

namespace Boilerplate.Web.Controllers
{
    /// <summary>
    /// Todo Controller
    /// </summary>
    public class TodoController : CommonControllerBase
    {
        private readonly BoilerplateContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public TodoController(BoilerplateContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get TodoItem collection.
        /// </summary>
        /// <returns>TodoItem collection</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /Todo
        ///     
        /// </remarks>
        /// <response code="200">Returns the todo item collection</response>
        [HttpGet]
        public async Task<ICollection<TodoItem>> Get()
        {
            return await _context.TodoItems.ToListAsync();
        }

        /// <summary>
        /// Get a TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A TodoItem</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Todo/1
        ///
        /// </remarks>
        /// <response code="200">Returns the todo item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="404">If the item is null</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem?>> Get(long id)
        {
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(item => item.Id == id);
            if(todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(TodoItem), typeof(TodoItemExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TodoItemExample))]
        // todo : Swagger에서 Antiforgery 사용가능한지 체크 해야 함
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = item.Id }, item);
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="400">If the item is null</response>
        /// <response code="406">If the item is null</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
