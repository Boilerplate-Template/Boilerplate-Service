using Boilerplate.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Web.Context
{
    /// <summary>
    /// Boilerplate DbContext
    /// </summary>
    public class BoilerplateContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public BoilerplateContext(DbContextOptions<BoilerplateContext> options) : base(options) {
        }

        /// <summary>
        /// Todo
        /// </summary>
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        /// <summary>
        /// On Configuring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(
            //    @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");

            optionsBuilder.UseSqlite("Data Source=BoilerplateData.db");
        }

        /// <summary>
        /// On Model Creating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
