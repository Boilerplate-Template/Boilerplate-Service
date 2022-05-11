using Boilerplate.Web.Context;
using Boilerplate.Web.Models;

namespace Boilerplate.Web.Data
{
    /// <summary>
    /// Db Initializer class
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context"></param>
        public static async Task InitializeAsync(BoilerplateContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Look for any todoItems.
            if (context.TodoItems.Any())
            {
                return;   // DB has been seeded
            }

            var todoItems = new[] { 
                new TodoItem() { Name = "Todo Item #1", IsComplete = true, Created = DateTime.Now, Description = "description #1" },
                new TodoItem() { Name = "Todo Item #2", IsComplete = false, Created = DateTime.Now, Description = "description #2" },
                new TodoItem() { Name = "Todo Item #3", IsComplete = false, Created = DateTime.Now, Description = "description #3" },
                new TodoItem() { Name = "Todo Item #4", IsComplete = false, Created = DateTime.Now, Description = "description #4" },
                new TodoItem() { Name = "Todo Item #5", IsComplete = false, Created = DateTime.Now, Description = "description #5" },
            };

            foreach(var todoItem in todoItems)
            {
                context.TodoItems.Add(todoItem);
            }

            await context.SaveChangesAsync();
        }
    }
}
