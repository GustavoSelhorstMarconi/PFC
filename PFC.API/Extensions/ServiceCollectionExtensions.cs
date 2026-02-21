using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PFC.API.Filters;
using System.Reflection;
using System.Text;

namespace PFC.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationActionFilter>();
        });

        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("ProductionPolicy", builder =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? new[] { "https://dialogo.com" };

                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PFC API",
                Version = "v1",
                Description = "API de controle pessoal financeiro com autenticação JWT",
                Contact = new OpenApiContact
                {
                    Name = "PFC Team",
                    Email = "contact@pfc.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: Bearer {seu token}"
            });

            options.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured.");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured.");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured.");

            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "name",
                RoleClaimType = "role"
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError("JWT Authentication failed: {Message}", context.Exception.Message);
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        logger.LogWarning("Token expirado!");
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("JWT Token validado com sucesso para usuário: {User}", context.Principal?.Identity?.Name);

                    var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}");
                    logger.LogInformation("Claims no token: {Claims}", string.Join(", ", claims ?? Array.Empty<string>()));

                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    var token = context.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        logger.LogInformation("Token recebido: {Token}", token.Substring(0, Math.Min(50, token.Length)) + "...");
                    }
                    else
                    {
                        logger.LogWarning("Nenhum token recebido no header Authorization");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("JWT Authentication challenge. Error: {Error}, ErrorDescription: {ErrorDescription}",
                        context.Error, context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddHttpContextAccessor();
        services.AddAuthorization();

        return services;
    }
}
