using Boilerplate.Web.Models;
using Boilerplate.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Web.Context;

/// <summary>
/// Boilerplate DbContext class
/// </summary>
public class BoilerplateContext : IdentityDbContext<IdentityWebUser>
{
    /// <summary>
    /// Todo
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    public BoilerplateContext(DbContextOptions<BoilerplateContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// On ModelCreating
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>().ToTable("TB_TodoItems");

        modelBuilder.Entity<IdentityWebUser>().ToTable("TB_Users");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("TB_RoleClaims");
        modelBuilder.Entity<IdentityRole>().ToTable("TB_Roles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("TB_UserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("TB_UserLogins");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("TB_UserRoles");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("TB_UserTokens");
    }

    /// <summary>
    /// On Configuring
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseSqlServer(
        //    @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");

        //optionsBuilder.UseSqlite("Data Source=BoilerplateData.db");

        base.OnConfiguring(optionsBuilder);
    }
}
