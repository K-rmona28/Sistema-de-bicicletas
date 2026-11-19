using ApiCycleOn.Data;
using ApiCycleOn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class MantenimientoController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public MantenimientoController(CycleOnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mantenimiento>>> GetMantenimientos()
        {
            return await _context.Mantenimientos.Include(m => m.IdBicicletaNavigation).ToListAsync();
        }

        // POST: api/Mantenimiento (RegistrarMantenimiento)
        [HttpPost]
        public async Task<ActionResult<Mantenimiento>> RegistrarMantenimiento([FromBody] Mantenimiento nuevoMantenimiento)
        {
            nuevoMantenimiento.Estado = "PROGRAMADO";
            _context.Mantenimientos.Add(nuevoMantenimiento);

            // Cambiar estado de la bicicleta a mantenimiento
            var bici = await _context.Bicicleta.FindAsync(nuevoMantenimiento.IdBicicleta);
            if (bici != null)
            {
                bici.Estado = "MANTENIMIENTO";
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMantenimiento), new { id = nuevoMantenimiento.IdMantenimiento }, nuevoMantenimiento);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Mantenimiento>> GetMantenimiento(int id)
        {
            var mant = await _context.Mantenimientos.Include(m => m.IdBicicletaNavigation).FirstOrDefaultAsync(m => m.IdMantenimiento == id);
            if (mant == null) return NotFound();
            return mant;
        }

        // PUT: api/Mantenimiento/5/finalizar (FinalizarMantenimiento)
        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> FinalizarMantenimiento(int id)
        {
            var mant = await _context.Mantenimientos.FindAsync(id);
            if (mant == null) return NotFound(new { mensaje = "Mantenimiento no encontrado." });

            mant.Estado = "FINALIZADO";

            // Devolver bicicleta a disponible
            var bici = await _context.Bicicleta.FindAsync(mant.IdBicicleta);
            if (bici != null)
            {
                bici.Estado = "DISPONIBLE";
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Mantenimiento finalizado y bicicleta habilitada." });
        }
    }
}