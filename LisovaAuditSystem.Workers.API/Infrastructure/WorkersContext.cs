using LisovaAuditSystem.Workers.API.Entities;

using Microsoft.EntityFrameworkCore;

namespace LisovaAuditSystem.Workers.API.Infrastructure;

public class WorkersContext(DbContextOptions<WorkersContext> options) : DbContext(options)
{
    public DbSet<Worker> Workers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Worker>(
            builder =>
            {
                const int maxLastNameLength = 64;
                const int maxNameLength = 64;

                const int maxPhoneLength = 12;
                const int maxEmailLength = 256;

                builder.ToTable("Workers");

                builder.HasKey(worker => worker.Id);

                builder.Property(worker => worker.Id)
                    .HasColumnOrder(1);

                builder.Property(worker => worker.LastName)
                    .HasMaxLength(maxLastNameLength)
                    .IsRequired();

                builder.Property(worker => worker.Name)
                    .HasMaxLength(maxNameLength)
                    .IsRequired();

                builder.Property(worker => worker.Phone)
                    .HasMaxLength(maxPhoneLength)
                    .IsRequired();

                builder.Property(worker => worker.Email)
                    .HasMaxLength(maxEmailLength)
                    .IsRequired(false);
            });

        base.OnModelCreating(modelBuilder);
    }
}
