using Microsoft.EntityFrameworkCore;
using task_management.Contexts;
using task_management.Models;
using task_management.Repositories;

namespace task_management_test.Repositories
{
    public class UsuarioRepositoryTests
    {
        private readonly TaskManagementContext _mockContext;
        private readonly UsuarioRepository _repository;

        public UsuarioRepositoryTests()
        {
            // Configurar contexto in-memory
            var options = new DbContextOptionsBuilder<TaskManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new TaskManagementContext(options);
            _repository = new UsuarioRepository(_mockContext);
        }

        [Fact]
        public async Task AdicionarAsync_DeveAdicionarUsuarioCorretamente()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "teste@email.com"
            };

            // Act
            var usuarioCriado = await _repository.AdicionarAsync(usuario);

            // Assert
            Assert.NotNull(usuarioCriado);
            Assert.Equal("Teste", usuarioCriado.Nome);
            Assert.Equal("teste@email.com", usuarioCriado.Email);
        }

        [Fact]
        public async Task EmailExisteAsync_DeveRetornarTrueParaEmailJaCadastrado()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "teste@email.com"
            };
            
            var usuarioCriado = await _repository.AdicionarAsync(usuario);
            
            // Act
            var resultado = await _repository.EmailExisteAsync("teste@email.com");

            // Assert
            Assert.True(resultado);
        }
    }
}
