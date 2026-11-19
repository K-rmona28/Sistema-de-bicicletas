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
    public class ReservasController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public ReservasController(CycleOnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas
                .Include(r => r.IdBicicletaNavigation)
                .Include(r => r.IdEstacionNavigation)
                .ToListAsync();
        }

        // POST: api/Reservas (CrearReserva)
        [HttpPost]
        public async Task<ActionResult<Reserva>> CrearReserva([FromBody] Reserva nuevaReserva)
        {
            nuevaReserva.Estado = "ACTIVA";
            nuevaReserva.FechaHoraReserva = DateTime.Now;

            _context.Reservas.Add(nuevaReserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = nuevaReserva.IdReserva }, nuevaReserva);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.IdBicicletaNavigation)
                .Include(r => r.IdEstacionNavigation)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound(new { mensaje = "Reserva no encontrada." });

            return reserva;
        }

        // PUT: api/Reservas/5/cancelar (CancelarReserva)
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound(new { mensaje = "Reserva no encontrada." });

            reserva.Estado = "CANCELADA";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Reserva cancelada exitosamente." });
        }

        // PUT: api/Reservas/5/terminar (TerminarReserva)
        [HttpPut("{id}/terminar")]
        public async Task<IActionResult> TerminarReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound(new { mensaje = "Reserva no encontrada." });

            reserva.Estado = "FINALIZADA";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Reserva finalizada exitosamente." });
        }
    }
}