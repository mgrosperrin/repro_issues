using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace ProductApplication;

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
                             //.AddMicrosoftGraph()
                             .AddInMemoryTokenCaches()
                             ;

        builder.Services.AddControllers();
        builder.Services.AddCors(corsOptions => corsOptions.AddDefaultPolicy(new CorsPolicyBuilder()
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build()));
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {{
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    } });
            var scopes = new Dictionary<string, string>
            {
                        { $"api://{ builder.Configuration.GetValue<string>("EntraId:MozzAPIClientId")}/.default", "Access application on user behalf" },
                        { $"api://{builder.Configuration.GetValue<string>("EntraId:ClientId")}/user_impersonation", "user_impersonation" },
                        { $"api://{builder.Configuration.GetValue<string>("EntraId:ClientId")}/.default", ".default" }
            };
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri("https://login.microsoftonline.com/organizations/oauth2/v2.0/token"),
                        Scopes = scopes
                    }
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthAppName("Swagger Client");
                options.OAuthClientId(builder.Configuration.GetValue<string>("EntraId:ClientId"));
                options.OAuthUsePkce();
            });
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
