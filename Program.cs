using Scalar.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Application.Extensions;
using CrossCutting.Extensions;
using CrossCutting.Logging;

// Bootstrap logger: captura erros antes do host estar configurado
Log.Logger = SerilogConfiguration.CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Serilog completo com CorrelationId, Application e Environment
builder.Host.UseApplicationSerilog();

try
{
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddApplicationServices();
    builder.Services.AddCrossCuttingServices(builder.Configuration);
    builder.Services.AddInfrastructureServices();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ECommerceDbContext>(options =>
        options.UseNpgsql(connectionString)
    );

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
        dbContext.Database.Migrate();
    }

    app.MapOpenApi();
    app.MapScalarApiReference();

    // CorrelationId ? ErrorHandling ? SerilogRequestLogging ? resto da pipeline
    app.UseCrossCuttingMiddlewares();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Iniciando aplicação E-commerce API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada com erro");
}
finally
{
    Log.CloseAndFlush();
}

