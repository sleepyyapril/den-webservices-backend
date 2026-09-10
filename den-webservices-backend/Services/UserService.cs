using DenWebServices.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace DenWebServices.Backend.Services;

public class UserService : IUserService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public UserService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<User> LoginAsync(CurrentSession session)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        if (!db.Users.Any(u => u.Email == session.EmailAddress))
            await RegisterAsync(session);
        
        var user = await db.Users.SingleAsync(u => u.Email == session.EmailAddress);
        return user;
    }

    public async Task RegisterAsync(CurrentSession session)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var user = new User
        {
            UniqueId = Guid.NewGuid(),
            Email = session.EmailAddress,
            Punishments = [],
            CreatedPunishments = [],
            DiscordId = session.DiscordId,
            Rank = null
        };
        
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
    }
}

public interface IUserService
{
    Task<User> LoginAsync(CurrentSession session);
    Task RegisterAsync(CurrentSession session);
}