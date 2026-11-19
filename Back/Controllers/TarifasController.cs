using ApiCycleOn.Data;
using ApiCycleOn.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCycleOn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifasController : ControllerBase
    {
        private readonly CycleOnContext _context;

        public TarifasController(CycleOnContext context)
        {
            _context = context;
        }

        // GET: api/Tarifas (MostrarTarifa)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarifa>>> GetTarifas()
        {
            return await _context.Tarifas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarifa>> MostrarTarifa(int id)
        {
            var tarifa = await _context.Tarifas.FindAsync(id);
            if (tarifa == null) return NotFound(new { mensaje = "Tarifa no encontrada." });
            return tarifa;
        }

        // PUT: api/Tarifas/5 (ModificarTarifa)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ModificarTarifa(int id, [FromBody] Tarifa tarifaActualizada)
        {
            if (id != tarifaActualizada.IdTarifa) return BadRequest();

            _context.Entry(tarifaActualizada).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Tarifa modificada exitosamente." });
        }

        // POST: api/Tarifas/calcular (CalcularValorServicio usando estrategia si aplica)
        [HttpPost("calcular")]
        public IActionResult CalcularValorServicio([FromBody] CalculoTarifaDto dto)
        {
            // Lógica base de cálculo por minutos de uso
            decimal costoMinuto = 100m; // Ejemplo
            decimal total = (decimal)dto.MinutosUso * costoMinuto;

            return Ok(new { minutosUso = dto.MinutosUso, costoTotal = total });
        }
    }

    public class CalculoTarifaDto
    {
        public int MinutosUso { get; set; }
    }
}