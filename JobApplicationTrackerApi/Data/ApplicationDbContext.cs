using JobApplicationTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<JobApplication> JobApplications { get; set; }
}