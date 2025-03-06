using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using task_management.Caches;
using task_management.Dtos;
using task_management.Models;
using task_management.Repositories;

namespace task_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IUsuarioCacheService _usuarioCacheService;
        private readonly ITarefaCacheService _tarefaCacheService;

        public UsuarioController(
            IUsuarioRepository usuarioRepository,
            ITarefaRepository tarefaRepository,
            IUsuarioCacheService usuarioCacheService,
            ITarefaCacheService tarefaCacheService)
        {
            _usuarioRepository = usuarioRepository;
            _tarefaRepository = tarefaRepository;
            _usuarioCacheService = usuarioCacheService;
            _tarefaCacheService = tarefaCacheService;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")] // Esta anotação exige autenticação e role específica
        public async Task<ActionResult<UsuarioDto>> CriarUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _usuarioRepository.EmailExisteAsync(usuario.Email))
                return Conflict("Email já cadastrado");

            var usuarioCriado = await _usuarioRepository.AdicionarAsync(usuario);

            return CreatedAtAction(
                nameof(ObterUsuario),
                new { id = usuarioCriado.Id },
                new UsuarioDto
                {
                    Id = usuarioCriado.Id,
                    Nome = usuarioCriado.Nome,
                    Email = usuarioCriado.Email
                }
            );
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> ObterUsuario(int id)
        {
            var usuario = await _usuarioCacheService.ObterPorIdComCacheAsync(id);

            if (usuario == null)
                return NotFound();

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }


        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> ListarUsuarios()
        {
            var usuarios = await _usuarioRepository.ListarTodosAsync();

            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email
            }).ToList();
        }

        [HttpPost("{id}/tarefas")]
        public async Task<ActionResult<Tarefa>> CriarTarefa(int id, [FromBody] CriarTarefaDto tarefaDto)
        {
            var usuario = await _usuarioCacheService.ObterPorIdComCacheAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            var tarefa = new Tarefa
            {
                Titulo = tarefaDto.Titulo,
                Descricao = tarefaDto.Descricao,
                Status = tarefaDto.Status,
                UsuarioId = id
            };

            var tarefaCriada = await _tarefaRepository.AdicionarAsync(tarefa);

            // Invalidar cache de tarefas do usuário
            await _tarefaCacheService.RemoverCacheTarefasAsync(id);

            return CreatedAtAction(
                nameof(ListarTarefas),
                new { id = usuario.Id },
                tarefaCriada
            );
        }

        [HttpGet("{id}/tarefas")]
        public async Task<ActionResult<List<Tarefa>>> ListarTarefas(int id)
        {
            var usuario = await _usuarioCacheService.ObterPorIdComCacheAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            return await _tarefaCacheService.ObterTarefasPorUsuarioComCacheAsync(id);
        }
    }
}