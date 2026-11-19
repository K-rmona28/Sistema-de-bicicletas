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
    public class FacturasController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public FacturasController(CycleOnContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            return await _context.Facturas.Include(f => f.IdUsuarioNavigation).ToListAsync();
        }

        // GET: api/Facturas/5 (ConsultarFactura)
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> ConsultarFactura(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.IdUsuarioNavigation)
                .Include(f => f.IdPagoNavigation)
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            if (factura == null) return NotFound(new { mensaje = "Factura no encontrada." });

            return factura;
        }

        // POST: api/Facturas (GenerarFactura)
        [HttpPost]
        public async Task<ActionResult<Factura>> GenerarFactura([FromBody] Factura nuevaFactura)
        {
            nuevaFactura.FechaEmision = DateTime.Now;
            _context.Facturas.Add(nuevaFactura);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ConsultarFactura), new { id = nuevaFactura.IdFactura }, nuevaFactura);
        }

        // GET: api/Facturas/5/exportar (ExportarFactura en PDF/Texto simulado)
        [HttpGet("{id}/exportar")]
        public async Task<IActionResult> ExportarFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null) return NotFound(new { mensaje = "Factura no encontrada." });

            // Simulación de exportación (retorna los datos listos para formatear en PDF desde Angular)
            return Ok(new
            {
                mensaje = "Factura lista para exportar",
                detalle = factura
            });
        }
    }
}