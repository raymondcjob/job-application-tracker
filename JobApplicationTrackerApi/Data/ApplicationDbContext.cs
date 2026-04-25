using JobApplicationTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Data;

public class ApplicationDbContext : DbContext
{
    public const string DefaultAdminIdentifier = "admin";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<JobApplication> JobApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(user => user.Identifier)
            .IsUnique();
    }
}
