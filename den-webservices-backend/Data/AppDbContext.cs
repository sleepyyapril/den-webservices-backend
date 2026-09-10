using Microsoft.EntityFrameworkCore;

namespace DenWebServices.Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Rank> Ranks { get; set; } = null!;
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Punishment> Punishments { get; set; } = null!;
    public DbSet<AssignedUserId> AssignedUserIds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(p => p.UniqueId)
            .IsUnique();
        
        modelBuilder.Entity<AssignedUserId>()
            .HasIndex(p => p.UserId)
            .IsUnique();
        
        modelBuilder.Entity<AssignedUserId>()
            .HasIndex(p => p.UserName)
            .IsUnique();

        modelBuilder.Entity<Punishment>()
            .HasOne(p => p.PunishingUser)
            .WithMany(p => p.CreatedPunishments)
            .HasPrincipalKey(p => p.UniqueId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Admin>()
            .HasOne(p => p.AdminRank)
            .WithMany(p => p.Admins)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Rank>()
            .HasMany(p => p.Admins)
            .WithOne(p => p.AdminRank)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<AdminFlag>()
            .HasIndex(f => new {f.Flag, f.AdminId})
            .IsUnique();

        modelBuilder.Entity<RankFlag>()
            .HasIndex(f => new {f.Flag, f.RankId})
            .IsUnique();
    }
}