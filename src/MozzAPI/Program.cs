using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Identity.Web;

namespace MozzAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("EntraId"))
                             .EnableTokenAcquisitionToCallDownstreamApi(
                                 options =>
                                 {
                                 })
                             .AddMicrosoftGraph()
                             .AddInMemoryTokenCaches()
                             ;

        builder.Services.AddControllers();
        builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(new CorsPolicyBuilder()
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build()));
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
