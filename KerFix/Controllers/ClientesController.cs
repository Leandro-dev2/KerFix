
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> ObtenerClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();

            return Ok(clientes);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return StatusCode(201, cliente);
        }
    }
}
