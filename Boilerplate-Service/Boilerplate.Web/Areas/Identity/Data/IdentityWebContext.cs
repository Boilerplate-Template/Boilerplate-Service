using Boilerplate.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Web.Data;

/// <summary>
/// IdentityWebContext class
/// </summary>
public class IdentityWebContext : IdentityDbContext<IdentityWebUser>
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    public IdentityWebContext(DbContextOptions<IdentityWebContext> options)
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

        modelBuilder.Entity<IdentityWebUser>().ToTable("User");
    }

    /// <summary>
    /// On Configuring
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
