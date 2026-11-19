using ApiCycleOn.Data;
using ApiCycleOn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicicletasController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public BicicletasController(CycleOnContext context)
        {
            _context = context;
        }

        // GET: api/Bicicletas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bicicletum>>> GetBicicletas()
        {
            return await _context.Bicicleta
                .Include(b => b.IdEstacionNavigation)
                .ToListAsync();
        }

        // GET: api/Bicicletas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bicicletum>> GetBicicleta(int id)
        {
            var bicicleta = await _context.Bicicleta
                .Include(b => b.IdEstacionNavigation)
                .FirstOrDefaultAsync(b => b.IdBicicleta == id);

            if (bicicleta == null)
            {
                return NotFound(new { mensaje = "Bicicleta no encontrada." });
            }

            return bicicleta;
        }

        // PUT: api/Bicicletas/5/estado (CambiarEstado, Bloquear o Mantenimiento)
        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Administrador")] // Solo administradores según el diagrama
        public async Task<IActionResult> CambiarEstadoBicicleta(int id, [FromBody] CambiarEstadoDto request)
        {
            var bicicleta = await _context.Bicicleta.FindAsync(id);
            if (bicicleta == null)
            {
                return NotFound(new { mensaje = "Bicicleta no encontrada." });
            }

            // Actualizamos el estado (ej: "DISPONIBLE", "MANTENIMIENTO", "BLOQUEADA", "EN_PRESTAMO")
            bicicleta.Estado = request.NuevoEstado;

            // Si viene opcionalmente una estación nueva o cambio de asignación
            if (request.IdEstacion.HasValue)
            {
                bicicleta.IdEstacion = request.IdEstacion.Value;
            }

            _context.Entry(bicicleta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BicicletaExists(id))
                {
                    return NotFound(new { mensaje = "La bicicleta ya no existe." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { mensaje = $"Estado de la bicicleta actualizado a '{request.NuevoEstado}' exitosamente.", bicicleta });
        }

        private bool BicicletaExists(int id)
        {
            return _context.Bicicleta.Any(e => e.IdBicicleta == id);
        }
    }

    // DTO auxiliar para recibir el cambio de estado de forma limpia
    public class CambiarEstadoDto
    {
        public string NuevoEstado { get; set; } = string.Empty;
        public int? IdEstacion { get; set; }
    }
}