using LisovaAuditSystem.Workers.API.Entities;

using Microsoft.EntityFrameworkCore;

namespace LisovaAuditSystem.Workers.API.Infrastructure;

public class WorkersContext(DbContextOptions<WorkersContext> options) : DbContext(options)
{
    public DbSet<Worker> Workers { get; set; }

    public DbSet<User> Users { get; set; }

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

        modelBuilder.Entity<User>(
            builder =>
            {
                const int maxUserNameLength = 16;
                const int maxEmailLength = 256;
                const int maxPasswordLength = 512;

                builder.ToTable("Users");

                builder.HasKey(user => user.Id);

                builder.Property(user => user.UserName)
                    .HasMaxLength(maxUserNameLength)
                    .IsRequired();

                builder.Property(user => user.Email)
                    .HasMaxLength(maxEmailLength)
                    .IsRequired();

                builder.Property(user => user.PasswordHash)
                    .HasMaxLength(maxPasswordLength)
                    .IsRequired();

                builder.HasIndex(user => user.UserName).IsUnique();
                builder.HasIndex(user => user.Email).IsUnique();
            });

        base.OnModelCreating(modelBuilder);
    }
}
