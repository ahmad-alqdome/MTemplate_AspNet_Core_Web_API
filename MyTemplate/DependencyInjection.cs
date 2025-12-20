using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyTemplate.Filters;
using MyTemplate.Persistense;
using MyTemplate.Services.Implementations;
using System.Reflection;


namespace MyTemplate;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(op=>op.Filters.Add<ValidationFilter>());
        services.AddFluentValidationConfig();
        services.AddOpenApi();
        services.AddJwtOption();
        services.AddAuthConfig(configuration);
        services.AddIdentityConfig(configuration);
        services.AddHttpContextAccessor();
        services.AddCORSConfig(configuration);

        services.AddServices();

        services.AddMapsterConfig();


        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtProvider,JwtProvider>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddJwtOption(this IServiceCollection services)
    {

        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }
    private static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
    private static IServiceCollection AddCORSConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(allowedOrigins!) // السماح لـ Angular
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials(); // ⚠️ لا تستخدم `AllowCredentials()` مع `AllowAnyOrigin()`
            });
        });
        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(config =>
        {
            config.SuppressModelStateInvalidFilter = true;
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // JwtBearerDefaults.AuthenticationScheme return "Bearer" string
        var authenticationScheme = JwtBearerDefaults.AuthenticationScheme; 

        // Get JwtOptions values from appsettings.json and bind to JwtOptions class
        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!; 


        // Create SymmetricSecurityKey using the secret key from JwtOptions
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

        // Token validation parameters for validating JWT tokens in incoming requests
        // ClockSkew  No Extra Time (default is 5 min) so tokens expire exactly at token expiration time
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, 
            IssuerSigningKey = symmetricSecurityKey,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience
        };

        // Configure authentication to use JWT Bearer tokens
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = authenticationScheme;
            options.DefaultChallengeScheme = authenticationScheme;
        }
        ).AddJwtBearer(option => option.TokenValidationParameters = tokenValidationParameters);

        return services;
    }
}
