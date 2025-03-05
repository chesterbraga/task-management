using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using task_management.Caches;
using task_management.Models;
using task_management.Repositories;

public class UsuarioCacheServiceTests
{
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<IUsuarioRepository> _mockRepository;
    private readonly Mock<ILogger<UsuarioCacheService>> _mockLogger;
    private readonly UsuarioCacheService _cacheService;

    public UsuarioCacheServiceTests()
    {
        _mockCache = new Mock<IDistributedCache>();
        _mockRepository = new Mock<IUsuarioRepository>();
        _mockLogger = new Mock<ILogger<UsuarioCacheService>>();

        _cacheService = new UsuarioCacheService(
            _mockCache.Object,
            _mockRepository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ObterPorIdComCacheAsync_DeveRetornarUsuarioDoCacheSeExistir()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nome = "Usuário Teste" };
        var usuarioSerializado = System.Text.Json.JsonSerializer.Serialize(usuario);

        _mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(Encoding.UTF8.GetBytes(usuarioSerializado));

        // Act
        var resultado = await _cacheService.ObterPorIdComCacheAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Usuário Teste", resultado.Nome);
    }
}