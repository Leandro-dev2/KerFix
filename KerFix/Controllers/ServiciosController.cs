using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerFix.Data;
using KerFix.Models;

namespace KerFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly KerFixDbContext _context;
        public ServiciosController(KerFixDbContext context)
        {
            _context = context;
        }

        //Metodo para Obtener el listado de todos los servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> ObtenerServicios()
        {
            var servicios = await _context.Servicios.ToListAsync();
            
            
            return Ok(servicios);
        }

        //Metodo para obtener el servicio con un ID especifico
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Servicio>> ObtenerServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio is null)
            {
                return NotFound();
            }
            return Ok(servicio);
        }

        //Metodo para crear un nuevo servicio
        [HttpPost]
        public async Task<ActionResult<Servicio>> CrearServicio(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(ObtenerServicio),
                new { id = servicio.Id },
                servicio
                );
        }

        //Metodo para editar un servicio especifico
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Servicio>> EditarServicio(int id, [FromBody] Servicio datos)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio is null)
            {
                return NotFound();
            }
            servicio.Tipo = datos.Tipo;
            servicio.Descripcion = datos.Descripcion;
            servicio.Valor = datos.Valor;
            await _context.SaveChangesAsync();
            return Ok(servicio);
        }

        //Metodo para eliminar un servicio especifico
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio is null)
            {
                return NotFound();
            }
            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
