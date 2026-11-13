
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerFix.Data;
using KerFix.Models;

namespace KerFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly KerFixDbContext _context;

        public InventarioController(KerFixDbContext context)
        {
            _context = context;
        }

        // Metodo para obtener todos los repuestos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioRepuesto>>> ObtenerInventario()
        {
            var inventario = await _context.InventarioRepuestos.ToListAsync();

            return Ok(inventario);
        }

        // Metodo para obtener un repuesto por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventarioRepuesto>> ObtenerRepuesto(int id)
        {
            var repuesto = await _context.InventarioRepuestos.FindAsync(id);

            if (repuesto is null)
            {
                return NotFound("Repuesto no encontrado");
            }

            return Ok(repuesto);
        }

        // Metodo para registrar un repuesto
        [HttpPost]
        public async Task<ActionResult<InventarioRepuesto>> CrearRepuesto(InventarioRepuesto repuesto)
        {
            if (repuesto.Precio < 0 || repuesto.Cantidad < 0)
            {
                return BadRequest("El precio y la cantidad no pueden ser negativos");
            }

            _context.InventarioRepuestos.Add(repuesto);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerRepuesto),
                new { id = repuesto.Id },
                repuesto
            );
        }

        // Metodo para modificar un repuesto
        [HttpPut("{id:int}")]
        public async Task<ActionResult<InventarioRepuesto>> EditarRepuesto(int id, [FromBody] InventarioRepuesto datos)
        {
            var repuesto = await _context.InventarioRepuestos.FindAsync(id);

            if (repuesto is null)
            {
                return NotFound("Repuesto no encontrado");
            }

            if (datos.Precio < 0 || datos.Cantidad < 0)
            {
                return BadRequest("El precio y la cantidad no pueden ser negativos");
            }

            repuesto.TipoRepuesto = datos.TipoRepuesto;
            repuesto.Especificacion = datos.Especificacion;
            repuesto.Capacidad = datos.Capacidad;
            repuesto.Precio = datos.Precio;
            repuesto.Cantidad = datos.Cantidad;

            await _context.SaveChangesAsync();

            return Ok(repuesto);
        }

        // Metodo para eliminar un repuesto
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarRepuesto(int id)
        {
            var repuesto = await _context.InventarioRepuestos.FindAsync(id);

            if (repuesto is null)
            {
                return NotFound("Repuesto no encontrado");
            }

            _context.InventarioRepuestos.Remove(repuesto);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
