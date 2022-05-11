using Swashbuckle.AspNetCore.Filters;

namespace Boilerplate.Web.Models
{
    /// <summary>
    /// TodoItem Example class
    /// </summary>
    public class TodoItemExample : IExamplesProvider<TodoItem>
    {
        /// <summary>
        /// Get Examples
        /// </summary>
        /// <returns></returns>
        public TodoItem GetExamples()
        {
            return new TodoItem
            {
                Name = "new todo item", Description = "새로운 할 일", Created = DateTime.Now, IsComplete = false
            };
        }
    }
}
