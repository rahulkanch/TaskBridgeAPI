using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Projects;

namespace TaskBridge.Api.Common.Persistence;

public sealed class TaskBridgeDbContext(DbContextOptions<TaskBridgeDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Name).HasMaxLength(200).IsRequired();
            entity.Property(project => project.Description).HasMaxLength(2000);
            entity.Property(project => project.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasIndex(project => project.TeamId);
        });
    }
}