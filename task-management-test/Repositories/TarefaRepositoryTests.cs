using Microsoft.EntityFrameworkCore;
using task_management.Contexts;
using task_management.Enums;
using task_management.Models;
using task_management.Repositories;

namespace task_management_test.Repositories
{
    public class TarefaRepositoryTests
    {
        private readonly TaskManagementContext _context;
        private readonly TarefaRepository _repository;

        public TarefaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagementContext>()
                .UseInMemoryDatabase(databaseName: "TarefaTestDatabase")
                .Options;

            _context = new TaskManagementContext(options);
            _repository = new TarefaRepository(_context);
        }

        [Fact]
        public async Task AdicionarAsync_DeveCriarTarefaParaUsuario()
        {
            // Arrange
            var tarefa = new Tarefa
            {
                Titulo = "Tarefa Teste",
                Descricao = "Teste",
                UsuarioId = 1,
                Status = StatusTarefa.Pendente
            };

            // Act
            var tarefaCriada = await _repository.AdicionarAsync(tarefa);

            // Assert
            Assert.NotNull(tarefaCriada);
            Assert.Equal("Tarefa Teste", tarefaCriada.Titulo);
            Assert.Equal(StatusTarefa.Pendente, tarefaCriada.Status);
        }
    }
}