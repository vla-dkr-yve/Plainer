using Microsoft.EntityFrameworkCore;
using Plainer.Entities;

namespace Plainer.Data;

public class PlainerDbContext(DbContextOptions<PlainerDbContext> options):DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    modelBuilder.Entity<Role>().HasData(
        new Role { Id = 1, Name = "Owner" },
        new Role { Id = 2, Name = "Manager" },
        new Role { Id = 3, Name = "Participant" }
    );

    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Work" },
        new Category { Id = 2, Name = "Personal" },
        new Category { Id = 3, Name = "Holiday" },
        new Category { Id = 4, Name = "Sport" }
    );

    modelBuilder.Entity<EventParticipant>().HasKey(ep => new { ep.UserId, ep.EventId });

    modelBuilder.Entity<EventParticipant>()
        .HasOne(ep => ep.User)
        .WithMany(u => u.EventParticipants)
        .HasForeignKey(ep => ep.UserId);

    modelBuilder.Entity<EventParticipant>()
        .HasOne(ep => ep.Event)
        .WithMany(e => e.EventParticipants)
        .HasForeignKey(ep => ep.EventId);

    modelBuilder.Entity<EventParticipant>()
        .HasOne(ep => ep.Role)
        .WithMany()
        .HasForeignKey(ep => ep.RoleId);

    modelBuilder.Entity<Event>()
        .HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.CreatedBy)
        .OnDelete(DeleteBehavior.Restrict);

    base.OnModelCreating(modelBuilder);
    }

}
