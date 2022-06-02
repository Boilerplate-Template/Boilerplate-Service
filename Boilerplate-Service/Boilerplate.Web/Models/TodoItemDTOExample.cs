using Swashbuckle.AspNetCore.Filters;

namespace Boilerplate.Web.Models
{
    /// <summary>
    /// TodoItem Example class
    /// </summary>
    public class TodoItemDTOExample : IExamplesProvider<TodoItemDTO>
    {
        /// <summary>
        /// Get Examples
        /// </summary>
        /// <returns></returns>
        public TodoItemDTO GetExamples()
        {
            return new TodoItemDTO
            {
                Name = "new todo item",
                Description = "새로운 할 일",
                Created = DateTime.Now,
                IsComplete = false
            };
        }
    }
}
