using Microsoft.AspNetCore.Mvc;
using Moq;
using task_management.Caches;
using task_management.Controllers;
using task_management.Dtos;
using task_management.Enums;
using task_management.Models;
using task_management.Repositories;

namespace task_management_test.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<ITarefaRepository> _mockTarefaRepository;
        private readonly Mock<IUsuarioCacheService> _mockUsuarioCacheService;
        private readonly Mock<ITarefaCacheService> _mockTarefaCacheService;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockTarefaRepository = new Mock<ITarefaRepository>();
            _mockUsuarioCacheService = new Mock<IUsuarioCacheService>();
            _mockTarefaCacheService = new Mock<ITarefaCacheService>();

            _controller = new UsuarioController(
                _mockUsuarioRepository.Object,
                _mockTarefaRepository.Object,
                _mockUsuarioCacheService.Object,
                _mockTarefaCacheService.Object
            );
        }

        [Fact]
        public async Task CriarUsuario_ComDadosValidos_DeveRetornarCreatedResult()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Novo Usuário",
                Email = "novo@email.com"
            };

            _mockUsuarioRepository.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockUsuarioRepository.Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) =>
                {
                    u.Id = 1;
                    return u;
                });

            // Act
            var resultado = await _controller.CriarUsuario(usuario);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var usuarioDto = Assert.IsType<UsuarioDto>(createdResult.Value);

            Assert.Equal("Novo Usuário", usuarioDto.Nome);
            Assert.Equal("novo@email.com", usuarioDto.Email);
            Assert.Equal(1, usuarioDto.Id);
        }

        [Fact]
        public async Task ObterUsuario_UsuarioExistente_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Usuário Existente",
                Email = "existente@email.com"
            };

            _mockUsuarioCacheService.Setup(c => c.ObterPorIdComCacheAsync(1))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _controller.ObterUsuario(1);

            // Assert            
            var usuarioDto = Assert.IsType<UsuarioDto>(resultado.Value);

            Assert.Equal(1, usuarioDto.Id);
            Assert.Equal("Usuário Existente", usuarioDto.Nome);
        }

        [Fact]
        public async Task ObterUsuario_UsuarioInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _mockUsuarioCacheService.Setup(c => c.ObterPorIdComCacheAsync(It.IsAny<int>()))
                .ReturnsAsync((Usuario)null);

            // Act
            var resultado = await _controller.ObterUsuario(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task CriarTarefa_UsuarioExistente_DeveRetornarCreatedResult()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Usuário Teste"
            };

            var tarefaDto = new CriarTarefaDto
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição da Tarefa",
                Status = StatusTarefa.Pendente
            };

            _mockUsuarioCacheService.Setup(c => c.ObterPorIdComCacheAsync(1))
                .ReturnsAsync(usuario);

            _mockTarefaRepository.Setup(r => r.AdicionarAsync(It.IsAny<Tarefa>()))
                .ReturnsAsync((Tarefa t) =>
                {
                    t.Id = 1;
                    return t;
                });

            // Act
            var resultado = await _controller.CriarTarefa(1, tarefaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var tarefa = Assert.IsType<Tarefa>(createdResult.Value);

            Assert.Equal("Nova Tarefa", tarefa.Titulo);
            Assert.Equal(StatusTarefa.Pendente, tarefa.Status);
            Assert.Equal(1, tarefa.UsuarioId);
        }
    }
}