using AutoMapper;
using AutoMapper.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
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
    public class TodoItemsController : CommonControllerBase
    {
        private readonly BoilerplateContext _context;

        /// <summary>
        /// TodoItemsController Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public TodoItemsController(BoilerplateContext context, ILogger<TodoItemsController> logger, IMapper mapper)
            : base(logger, mapper)
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
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> Get()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            return await _context.TodoItems
                .ProjectTo<TodoItemDTO>(this.Mapper.ConfigurationProvider)
                .ToListAsync();
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
        public async Task<ActionResult<TodoItemDTO?>> Get(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            //var todoItem = await _context.TodoItems.FindAsync(id);
            var todoItemDto = await _context.TodoItems.ProjectTo<TodoItemDTO>(this.Mapper.ConfigurationProvider).FirstOrDefaultAsync(item => item.Id == id);
            if (todoItemDto == null)
            {
                return NotFound();
            }

            return todoItemDto;
        }

        /// <summary>
        /// Create a TodoItem.
        /// </summary>
        /// <param name="todoItemDto"></param>
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
        [SwaggerRequestExample(typeof(TodoItemDTO), typeof(TodoItemDTOExample))]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(TodoItemDTOExample))]
        // todo : Swagger에서 Antiforgery 사용가능한지 체크 해야 함
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<TodoItemDTO?>> Create(TodoItemDTO todoItemDto)
        {
            var todoItem = this.Mapper.Map<TodoItem>(todoItemDto);
            todoItem.Created = DateTime.Now;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = todoItem.Id },
                this.Mapper.Map<TodoItemDTO>(todoItem));
        }

        /// <summary>
        /// Update a TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todoItemDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(TodoItemDTO), typeof(TodoItemDTOExample))]        
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(long id, TodoItemDTO todoItemDto)
        {
            if (id != todoItemDto.Id)
            {
                return BadRequest();
            }

            // 올려진 값 그대로 저장할 때 사용
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            // 데이터 수정
            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;
            todoItem.Description = todoItemDto.Description;
            todoItem.Updated = DateTime.Now;

            // 업데이트를 해야 한다고 플래그 변경
            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!Exists(id))
            {
                return NotFound();
            }

            return NoContent();
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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            var item = await _context.TodoItems.FindAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Todo Exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool Exists(long id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// TodoItem convert to TodoItemDTO 
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete,
                Description = todoItem.Description,
                Updated = todoItem.Updated,
                Created = todoItem.Created,
                Deleted = todoItem.Deleted
            };
    }
}
