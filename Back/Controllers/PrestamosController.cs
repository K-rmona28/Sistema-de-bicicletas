using ApiCycleOn.Data;
using ApiCycleOn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrestamosController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public PrestamosController(CycleOnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPrestamos()
        {
            return await _context.Prestamos
                .Include(p => p.IdBicicletaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.EstacionOrigenNavigation)
                .Include(p => p.EstacionDestinoNavigation)
                .ToListAsync();
        }

        // POST: api/Prestamos (Iniciar un préstamo)
        [HttpPost]
        public async Task<ActionResult<Prestamo>> IniciarPrestamo([FromBody] Prestamo nuevoPrestamo)
        {
            nuevoPrestamo.Estado = "ACEPTADO";
            nuevoPrestamo.FechaHoraInicio = DateTime.Now;

            _context.Prestamos.Add(nuevoPrestamo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrestamo), new { id = nuevoPrestamo.IdPrestamo }, nuevoPrestamo);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.IdBicicletaNavigation)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(p => p.IdPrestamo == id);

            if (prestamo == null) return NotFound(new { mensaje = "Préstamo no encontrado." });

            return prestamo;
        }

        // PUT: api/Prestamos/5/finalizar (Finalizar Préstamo)
        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> FinalizarPrestamo(int id, [FromBody] FinalizarPrestamoDto dto)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound(new { mensaje = "Préstamo no encontrado." });

            prestamo.FechaHoraFin = DateTime.Now;
            prestamo.EstacionDestino = dto.EstacionDestinoId;
            prestamo.Estado = "FINALIZADO";

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Préstamo finalizado con éxito.", prestamo });
        }
    }

    public class FinalizarPrestamoDto
    {
        public int EstacionDestinoId { get; set; }
    }
}