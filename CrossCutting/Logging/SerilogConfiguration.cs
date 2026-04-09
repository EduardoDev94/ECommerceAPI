using Microsoft.Extensions.Hosting;
using Serilog;

namespace CrossCutting.Logging;

/// <summary>
/// Centraliza a configuração do Serilog.
/// O template inclui {CorrelationId}, que é preenchido automaticamente pelo
/// CorrelationIdMiddleware via LogContext.PushProperty a cada requisição.
/// </summary>
public static class SerilogConfiguration
{
    // Template padrão — {CorrelationId} será "-" quando não estiver em contexto de requisição
    private const string ConsoleTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    private const string FileTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Logger mínimo para capturar erros durante o bootstrap da aplicação,
    /// antes do host estar configurado. Deve ser chamado antes de CreateBuilder().
    /// </summary>
    public static ILogger CreateBootstrapLogger() =>
        new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: ConsoleTemplate)
            .CreateLogger();

    /// <summary>
    /// Configura o Serilog completo via IHostBuilder.
    /// Enriquece todos os logs com: CorrelationId (via LogContext), Application e Environment.
    /// Quando "NewRelic:LicenseKey" estiver configurado, envia os logs também ao New Relic.
    /// </summary>
    public static IHostBuilder UseApplicationSerilog(this IHostBuilder host) =>
        host.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ECommerceAPI")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console(outputTemplate: ConsoleTemplate)
                .WriteTo.File(
                    path: "logs/ecommerce-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: FileTemplate);

            var licenseKey = context.Configuration["NewRelic:LicenseKey"];
            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                config.WriteTo.NewRelicLogs(
                    endpointUrl: "https://log-api.newrelic.com/log/v1",
                    applicationName: "ECommerceAPI",
                    licenseKey: licenseKey);
            }
        });
}
