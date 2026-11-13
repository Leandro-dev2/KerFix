
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerFix.Data;
using KerFix.Models;

namespace KerFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrosController : ControllerBase
    {
        private readonly KerFixDbContext _context;

        public RegistrosController(KerFixDbContext context)
        {
            _context = context;
        }

        // Obtener todos los registros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registro>>> ObtenerRegistros()
        {
            var registros = await _context.Registros.ToListAsync();

            return Ok(registros);
        }

        // Obtener un registro por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Registro>> ObtenerRegistro(int id)
        {
            var registro = await _context.Registros.FindAsync(id);

            if (registro is null)
            {
                return NotFound("Registro no encontrado");
            }

            return Ok(registro);
        }


        // Registrar un equipo
        [HttpPost]
        public async Task<ActionResult<Registro>> CrearRegistro(Registro registro)
        {
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Id == registro.IdCliente);

            if (!clienteExiste)
            {
                return BadRequest("El cliente indicado no existe");
            }

            // Fecha y hora automática de recepción
            registro.FechaRecepcion = DateTime.UtcNow;

            // Estado inicial del equipo
            registro.Estado = "Pendiente";

            _context.Registros.Add(registro);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerRegistro),
                new { id = registro.Id },
                registro
            );
        }


        // Modificar un registro
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Registro>> EditarRegistro(
            int id, [FromBody] Registro datos)
        {
            var registro = await _context.Registros.FindAsync(id);

            if (registro is null)
            {
                return NotFound("Registro no encontrado");
            }

            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Id == datos.IdCliente);

            if (!clienteExiste)
            {
                return BadRequest("El cliente indicado no existe");
            }

            registro.IdCliente = datos.IdCliente;
            registro.FechaRecepcion = datos.FechaRecepcion;
            registro.Tipo = datos.Tipo;
            registro.Marca = datos.Marca;
            registro.Modelo = datos.Modelo;
            registro.Serie = datos.Serie;
            registro.DiagnosticoCliente = datos.DiagnosticoCliente;
            registro.Estado = datos.Estado;

            await _context.SaveChangesAsync();

            return Ok(registro);
        }

        // Eliminar un registro
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarRegistro(int id)
        {
            var registro = await _context.Registros.FindAsync(id);

            if (registro is null)
            {
                return NotFound("Registro no encontrado");
            }

            _context.Registros.Remove(registro);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
