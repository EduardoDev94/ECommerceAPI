using Application.Services.Interfaces;
using CrossCutting.Auth;
using CrossCutting.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace CrossCutting.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Registra os middlewares do CrossCutting, JWT e o TokenService no container de DI.
    /// </summary>
    public static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<ErrorHandlingMiddleware>();

        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSection);
        services.AddScoped<ITokenService, JwtTokenService>();

        var jwtSettings = jwtSection.Get<JwtSettings>()!;
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidAudience            = jwtSettings.Audience,
                    IssuerSigningKey         = new SymmetricSecurityKey(key)
                };
            });

        return services;
    }

    /// <summary>
    /// Adiciona os middlewares na pipeline na ordem correta:
    ///   1. CorrelationId  → gera o ID e enriquece o LogContext
    ///   2. ErrorHandling  → captura exceções (já com CorrelationId disponível)
    ///   3. SerilogRequest → log estruturado de cada requisição HTTP
    /// Deve ser chamado antes de UseHttpsRedirection / UseAuthorization.
    /// </summary>
    public static IApplicationBuilder UseCrossCuttingMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} respondido {StatusCode} em {Elapsed:0.0000} ms | CorrelationId: {CorrelationId}";

            opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("CorrelationId",
                    httpContext.Items[CorrelationIdMiddleware.HeaderName] ?? "N/A");
                diagnosticContext.Set("UserAgent",
                    httpContext.Request.Headers.UserAgent.ToString());
            };
        });

        return app;
    }
}
