using Application.Mappings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerceAPI.UnitTests.Fixtures;

/// <summary>
/// Fixture compartilhada entre testes que precisam de AutoMapper real (não mockado).
/// Usa ServiceCollection para registrar todos os profiles via AddAutoMapper.
/// </summary>
public sealed class AutoMapperFixture
{
    public IMapper Mapper { get; }

    public AutoMapperFixture()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserMappingProfile).Assembly));
        var provider = services.BuildServiceProvider();
        Mapper = provider.GetRequiredService<IMapper>();
    }
}
