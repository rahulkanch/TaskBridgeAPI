using Microsoft.EntityFrameworkCore;
using TaskBridge.Api.Notifications;
using TaskBridge.Api.Projects;

namespace TaskBridge.Api.Common.Persistence;

public sealed class TaskBridgeDbContext(DbContextOptions<TaskBridgeDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.Id);
            entity.HasIndex(project => new { project.OrganisationId, project.TeamId });
            entity.Property(project => project.Name).HasMaxLength(200).IsRequired();
            entity.Property(project => project.Description).HasMaxLength(2000);
            entity.Property(project => project.Status).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(notification => notification.Id);
            entity.Property(notification => notification.Type).HasConversion<string>().HasMaxLength(64);
            entity.Property(notification => notification.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(notification => notification.Title).HasMaxLength(200).IsRequired();
            entity.Property(notification => notification.Body).HasMaxLength(4000).IsRequired();
            entity.HasIndex(notification => new
            {
                notification.OrganisationId,
                notification.RecipientUserId,
                notification.CreatedAt
            });
        });

        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.HasKey(record => record.Id);
            entity.Property(record => record.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(record => record.Action).HasMaxLength(100).IsRequired();
            entity.Property(record => record.PreviousStateJson).HasColumnType("TEXT");
            entity.Property(record => record.NewStateJson).HasColumnType("TEXT");
            entity.HasIndex(record => new
            {
                record.OrganisationId,
                record.EntityType,
                record.EntityId,
                record.OccurredAt
            });
        });
    }
}