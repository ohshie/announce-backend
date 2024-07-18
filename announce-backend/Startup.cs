using System.Text;
using announce_backend.Business.Auth.Authentification;
using announce_backend.Business.Auth.Authorization;
using announce_backend.Business.VkVideo;
using announce_backend.DAL.AnnounceDbContext;
using announce_backend.DAL.Repository;
using announce_backend.DAL.Repository.IRepository;
using announce_backend.DAL.UnitOfWork;
using announce_backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetSection("Jwt").GetValue<string>("Issuer"),
                    ValidAudience = builder.Configuration.GetSection("Jwt").GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt").GetValue<string>("Key")))
                })
            .AddScheme<AuthenticationSchemeOptions, Authenticator>("CreateUser", options => { });
        
        // dal
        builder.Services.AddScoped<IUnitOfWork<AnnounceDbContext>, UnitOfWork<AnnounceDbContext>>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IVkTokenRepository, VkTokenRepository>();
        
        // business
        builder.Services.AddTransient<AuthManager>();
        builder.Services.AddTransient<VkVideoUrlFetcher>();
        builder.Services.AddTransient<VkTokenManager>();
        
        builder.Services.AddSerilog();

        builder.Services.AddControllers();
            
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });
            
            c.AddSecurityDefinition("CreateUser", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                Description = "For user creation"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "JWT"
                        }
                    },
                    Array.Empty<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "CreateUser"
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