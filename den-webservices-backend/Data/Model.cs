using System.ComponentModel.DataAnnotations;

namespace DenWebServices.Backend.Data;

public class User
{
    public int Id { get; set; }
    public Guid UniqueId { get; set; }
    public Rank? Rank { get; set; }
    
    [MaxLength(128)]
    public string Email { get; set; } = "";
    public long DiscordId { get; set; }

    public List<Punishment> Punishments { get; set; } = default!;
    public List<Punishment> CreatedPunishments { get; set; } = default!;
}

public class AssignedUserId
{
    public int Id { get; set; }
    
    [MaxLength(32)]
    public string UserName { get; set; } = null!;
    
    public Guid UserId { get; set; }
}

public class Punishment
{
    public int Id { get; set; }
    public User User { get; set; } = default!;
    public User PunishingUser { get; set; } = default!;
    public Admin PunishingAdmin { get; set; } = default!;
    
    [MaxLength(32)]
    public string? PunishmentType { get; set; }
    public long? Start { get; set; }
    public long? Duration { get; set; }
    
    [MaxLength(128)]
    public string? Reason { get; set; }
}

public class Rank
{
    public int Id { get; set; }
    [MaxLength(32)]
    public string Name { get; set; } = "";

    public List<Admin> Admins { get; set; } = default!;
    public List<RankFlag> Flags { get; set; } = default!;
}

public class Admin
{
    public int Id { get; set; }
    public User User { get; set; }
    
    [MaxLength(32)]
    public string? Title { get; set; }
    public bool Suspended { get; set; }
    public Rank? AdminRank { get; set; }
    
    public List<AdminFlag> Flags { get; set; } = default!;
}

public class AdminFlag
{
    public int Id { get; set; }
    
    [MaxLength(64)]
    public string Flag { get; set; } = default!;
    public bool Negative { get; set; }
    public int AdminId { get; set; }
    public Admin Admin { get; set; } = default!;
}

public class RankFlag
{
    public int Id { get; set; }
    
    [MaxLength(64)]
    public string Flag { get; set; } = default!;
    public int RankId { get; set; }
    public Rank Rank { get; set; } = default!;
}
