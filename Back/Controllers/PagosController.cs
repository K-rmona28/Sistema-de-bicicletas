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
    public class PagosController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public PagosController(CycleOnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            return await _context.Pagos.Include(p => p.IdPrestamoNavigation).ToListAsync();
        }

        // POST: api/Pagos (RegistrarPago)
        [HttpPost]
        public async Task<ActionResult<Pago>> RegistrarPago([FromBody] Pago nuevoPago)
        {
            nuevoPago.Estado = "PENDIENTE";
            nuevoPago.FechaHoraPago = DateTime.Now;

            _context.Pagos.Add(nuevoPago);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPago), new { id = nuevoPago.IdPago }, nuevoPago);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> GetPago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound(new { mensaje = "Pago no encontrado." });
            return pago;
        }

        // PUT: api/Pagos/5/cancelar (CancelarPago)
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarPago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound(new { mensaje = "Pago no encontrado." });

            pago.Estado = "CANCELADO";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Pago cancelado con éxito." });
        }

        // POST: api/Pagos/procesar (ProcesarPagos masivos o individuales)
        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarPagos()
        {
            var pagosPendientes = await _context.Pagos.Where(p => p.Estado == "PENDIENTE").ToListAsync();

            foreach (var pago in pagosPendientes)
            {
                pago.Estado = "APROBADO"; // Lógica de pasarela de pago simulada
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Se procesaron {pagosPendientes.Count} pagos exitosamente." });
        }
    }
}