
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerFix.Data;
using KerFix.Models;

namespace KerFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly KerFixDbContext _context;

        public ClientesController(KerFixDbContext context)
        {
            _context = context;
        }

        //Metodo para Obtener el listado de todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> ObtenerClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();

            return Ok(clientes);
        }

        //Metodo para obtener el cliente con un ID especifico
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> ObtenerCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        //Metodo para crear un nuevo cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerCliente),
                new { id = cliente.Id },
                cliente
                );
        }

        //Metodo para editar un cliente especifico
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Cliente>> EditarCliente(int id, [FromBody] Cliente datos)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
            {
                return NotFound("Cliente no encontrado");
            }
            cliente.Nombre = datos.Nombre;
            cliente.Telefono = datos.Telefono;

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        //Metodo para eliminar un Cliente
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Cliente>> BorrarCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if ( cliente is null)
            {
                return NotFound("Cliente no encontrado");

            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
