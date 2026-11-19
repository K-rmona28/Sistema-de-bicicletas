using ApiCycleOn.Data;
using ApiCycleOn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstacionesController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public EstacionesController(CycleOnContext context)
        {
            _context = context;
        }

        // GET: api/Estaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estacion>>> GetEstaciones()
        {
            return await _context.Estacions.ToListAsync();
        }

        // GET: api/Estaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estacion>> GetEstacion(int id)
        {
            var estacion = await _context.Estacions
                .Include(e => e.Bicicleta) // Para ver la relación con bicicletas si está mapeada
                .FirstOrDefaultAsync(e => e.IdEstacion == id);

            if (estacion == null)
            {
                return NotFound(new { mensaje = "Estación no encontrada." });
            }

            return estacion;
        }

        // GET: api/Estaciones/5/bicicletas-activas
        [HttpGet("{id}/bicicletas-activas")]
        public async Task<IActionResult> VerBicicletasActivas(int id)
        {
            var bicicletasActivas = await _context.Bicicleta
                .Where(b => b.IdEstacion == id && b.Estado == "DISPONIBLE")
                .ToListAsync();

            return Ok(bicicletasActivas);
        }

        // POST: api/Estaciones (Solo administradores)
        [HttpPost]
        [Authorize(Roles = "Administrador")] // O el rol que manejes
        public async Task<ActionResult<Estacion>> AgregarEstacion([FromBody] Estacion nuevaEstacion)
        {
            _context.Estacions.Add(nuevaEstacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEstacion), new { id = nuevaEstacion.IdEstacion }, nuevaEstacion);
        }
    }
}