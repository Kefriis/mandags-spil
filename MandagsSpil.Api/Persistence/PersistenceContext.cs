using System;
using MandagsSpil.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MandagsSpil.Api.Persistence;

public class PersistenceContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<PlayerResult> PlayerResults { get; set; }

     public PersistenceContext(DbContextOptions<PersistenceContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.IdentityId).IsUnique();
            entity.Property(e => e.IdentityId).IsRequired();

             // Add this line to create a non-unique index on Cod2Username
            entity.HasIndex(e => e.Cod2Username);

            // Configure the one-to-many relationship
            entity.HasMany(u => u.PlayerResults)
                  .WithOne(pr => pr.User)
                  .HasForeignKey(pr => pr.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // A good idea for this relationship
        });

        // Configure PlayerResult entity
        modelBuilder.Entity<PlayerResult>(entity =>
        {
            entity.ToTable("PlayerResults");
            entity.HasKey(pr => pr.PlayerResultId);
            entity.Property(pr => pr.Cod2Username).IsRequired();
            entity.Property(pr => pr.Nation).HasConversion<string>();
        });

        // Configure Game entity (no changes needed for this relationship)
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Games");
            entity.HasKey(g => g.GameId);
            entity.HasMany(g => g.PlayerResults)
                  .WithOne(pr => pr.Game)
                  .HasForeignKey(pr => pr.GameId);
            entity.Property(g => g.WinningTeam).HasConversion<string>().IsRequired(false);
        });
    }
}
