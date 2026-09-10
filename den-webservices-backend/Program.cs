using DenWebServices.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace DenWebServices.Backend;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers();
        var app = builder.Build();

        app.MapControllers();
        app.Run();
    }
}