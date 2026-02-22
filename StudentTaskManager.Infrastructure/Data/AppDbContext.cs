using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;
using StudentTaskManager.Domain.Entities;

namespace StudentTaskManager.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
            b.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<TaskItem>(b =>
        {
            b.Property(t => t.Title).HasMaxLength(200).IsRequired();
            b.HasQueryFilter(t => !t.IsDeleted);

            b.HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(t => t.AssignedToUserId);
            b.HasIndex(t => t.DueDateUtc);
            b.HasIndex(t => t.Status);
        });
    }
}
