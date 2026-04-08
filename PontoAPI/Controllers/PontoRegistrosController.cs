using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PontoAPI.Models;

namespace PontoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PontoRegistrosController : ControllerBase
    {
        private readonly PontoContext _context;

        public PontoRegistrosController(PontoContext context)
        {
            _context = context;
        }

        // GET: api/PontoRegistros  → Lista todos os registros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PontoRegistro>>> GetRegistros()
        {
            return await _context.Registros.ToListAsync();
        }

        // GET: api/PontoRegistros/5  → Busca por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PontoRegistro>> GetPontoRegistro(long id)
        {
            var pontoRegistro = await _context.Registros.FindAsync(id);

            if (pontoRegistro == null)
                return NotFound();

            return pontoRegistro;
        }

        // POST: api/PontoRegistros  → Cria novo check-in (o mais importante)
        [HttpPost]
        public async Task<ActionResult<PontoRegistro>> PostPontoRegistro(PontoRegistro pontoRegistro)
        {
            _context.Registros.Add(pontoRegistro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPontoRegistro),
                new { id = pontoRegistro.Id }, pontoRegistro);
        }

        // DELETE: api/PontoRegistros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePontoRegistro(long id)
        {
            var pontoRegistro = await _context.Registros.FindAsync(id);
            if (pontoRegistro == null)
                return NotFound();

            _context.Registros.Remove(pontoRegistro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}