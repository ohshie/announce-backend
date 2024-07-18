using announce_backend.Business.Auth.Authentification;
using announce_backend.Business.Auth.Authorization;
using announce_backend.DAL.AnnounceDbContext;
using announce_backend.DAL.Repository;
using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace announce_backend;

public class Startup
{
    public WebApplication ConfigureServices()
    {
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddDbContext<AnnounceDbContext>(s =>
        {
            if (builder.Environment.IsEnvironment("Production"))
            {
                s.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                s.UseSqlite(builder.Configuration.GetConnectionString("Debug"));
            }
            
        });
        
        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Authentication"));
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddScheme<AuthenticationSchemeOptions, Authenticator>("Bearer", _ => { });
        
        // dal
        builder.Services.AddScoped<IUnitOfWork<AnnounceDbContext>, UnitOfWork<AnnounceDbContext>>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        
        // business
        builder.Services.AddTransient<AuthManager>();
        
        builder.Services.AddSerilog();

        builder.Services.AddControllers();
            
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", new CorsPolicyBuilder()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build());
        });

        return builder.Build();
    }
}