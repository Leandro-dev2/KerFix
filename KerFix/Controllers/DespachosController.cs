
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KerFix.Data;
using KerFix.Models;
using System.Data;

namespace KerFix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DespachosController : ControllerBase
    {
        private readonly KerFixDbContext _context;

        public DespachosController(KerFixDbContext context)
        {
            _context = context;
        }

        // CONSULTAR TODOS LOS DESPACHOS
        [HttpGet]
        public async Task<IActionResult> ObtenerDespachos()
        {
            var despachos = await _context.Despachos
                .AsNoTracking()
                .Select(d => new
                {
                    d.Id,
                    d.RegistroId,
                    d.FechaTermino,
                    d.ObsRecom,
                    d.ValorTotal,
                    Detalles = _context.DespachoDetalles
                        .Where(det => det.DespachoId == d.Id)
                        .ToList()
                })
                .ToListAsync();

            return Ok(despachos);
        }

        // CONSULTAR DESPACHO POR ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerDespacho(int id)
        {
            var despacho = await _context.Despachos
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new
                {
                    d.Id,
                    d.RegistroId,
                    d.FechaTermino,
                    d.ObsRecom,
                    d.ValorTotal,
                    Detalles = _context.DespachoDetalles
                        .Where(det => det.DespachoId == d.Id)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (despacho == null)
                return NotFound("Despacho no encontrado");

            return Ok(despacho);
        }

        // CREAR DESPACHO
        [HttpPost]
        public async Task<IActionResult> CrearDespacho(
            [FromBody] GuardarDespachoRequest datos)
        {
            var error = ValidarDetalles(datos.Detalles);
            if (error != null)
                return BadRequest(error);

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var registro = await _context.Registros
                    .FindAsync(datos.RegistroId);

                if (registro == null)
                    return BadRequest("El equipo no está registrado");

                if (registro.Estado == "Completado")
                    return BadRequest("Este equipo ya está completado");

                if (await _context.Despachos
                    .AnyAsync(d => d.RegistroId == datos.RegistroId))
                    return BadRequest("Este equipo ya tiene un despacho");

                var despacho = new Despacho
                {
                    RegistroId = datos.RegistroId,
                    FechaTermino = DateTime.Now,
                    ObsRecom = datos.ObsRecom,
                    ValorTotal = 0
                };

                _context.Despachos.Add(despacho);
                await _context.SaveChangesAsync();

                despacho.ValorTotal =
                    await AgregarDetalles(despacho.Id, datos.Detalles);

                registro.Estado = "Completado";

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return CreatedAtAction(
                    nameof(ObtenerDespacho),
                    new { id = despacho.Id },
                    new
                    {
                        despacho.Id,
                        despacho.RegistroId,
                        despacho.ValorTotal,
                        Mensaje = "Despacho creado correctamente"
                    });
            }
            catch (ArgumentException ex)
            {
                await transaccion.RollbackAsync();
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                return StatusCode(500,
                    "No se pudo crear el despacho");
            }
        }

        // EDITAR DESPACHO
        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarDespacho(
            int id,
            [FromBody] EditarDespachoRequest datos)
        {
            var error = ValidarDetalles(datos.Detalles);
            if (error != null)
                return BadRequest(error);

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var despacho = await _context.Despachos.FindAsync(id);

                if (despacho == null)
                    return NotFound("Despacho no encontrado");

                var detallesAnteriores = await _context.DespachoDetalles
                    .Where(d => d.DespachoId == id)
                    .ToListAsync();

                await DevolverInventario(detallesAnteriores);

                _context.DespachoDetalles.RemoveRange(detallesAnteriores);
                await _context.SaveChangesAsync();

                despacho.ObsRecom = datos.ObsRecom;

                despacho.ValorTotal =
                    await AgregarDetalles(id, datos.Detalles);

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return Ok(new
                {
                    despacho.Id,
                    despacho.ValorTotal,
                    Mensaje = "Despacho actualizado correctamente"
                });
            }
            catch (ArgumentException ex)
            {
                await transaccion.RollbackAsync();
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                return StatusCode(500,
                    "No se pudo actualizar el despacho");
            }
        }

        // ANULAR DESPACHO
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> AnularDespacho(int id)
        {
            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var despacho = await _context.Despachos.FindAsync(id);

                if (despacho == null)
                    return NotFound("Despacho no encontrado");

                var detalles = await _context.DespachoDetalles
                    .Where(d => d.DespachoId == id)
                    .ToListAsync();

                await DevolverInventario(detalles);

                var registro = await _context.Registros
                    .FindAsync(despacho.RegistroId);

                if (registro != null)
                    registro.Estado = "En revision";

                _context.DespachoDetalles.RemoveRange(detalles);
                _context.Despachos.Remove(despacho);

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return Ok(new
                {
                    Mensaje = "Despacho anulado correctamente",
                    InventarioRestaurado = true
                });
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                return StatusCode(500,
                    "No se pudo anular el despacho");
            }
        }

        // VALIDAR SERVICIOS Y REPUESTOS
        private static string? ValidarDetalles(
            List<DetalleRequest>? detalles)
        {
            if (detalles == null || detalles.Count == 0)
                return "Debes agregar al menos un servicio o repuesto";

            foreach (var item in detalles)
            {
                if (item.Cantidad <= 0)
                    return "La cantidad debe ser mayor a cero";

                bool repuesto = item.InventarioRepuestoId.HasValue;
                bool servicio = item.ServicioId.HasValue;

                if (repuesto == servicio)
                    return "Cada detalle debe indicar un servicio o un repuesto";
            }

            return null;
        }

        // AGREGAR DETALLES Y DESCONTAR INVENTARIO
        private async Task<decimal> AgregarDetalles(
            int despachoId,
            List<DetalleRequest> detalles)
        {
            decimal total = 0;

            foreach (var item in detalles)
            {
                decimal precio;

                if (item.InventarioRepuestoId.HasValue)
                {
                    var repuesto = await _context.InventarioRepuestos
                        .FindAsync(item.InventarioRepuestoId.Value);

                    if (repuesto == null)
                        throw new ArgumentException(
                            "Repuesto no encontrado");

                    if (repuesto.Cantidad < item.Cantidad)
                        throw new ArgumentException(
                            "Inventario insuficiente");

                    precio = repuesto.Precio;
                    repuesto.Cantidad -= item.Cantidad;
                }
                else
                {
                    var servicio = await _context.Servicios
                        .FindAsync(item.ServicioId!.Value);

                    if (servicio == null)
                        throw new ArgumentException(
                            "Servicio no encontrado");

                    precio = servicio.Valor;
                }

                decimal subtotal = precio * item.Cantidad;

                _context.DespachoDetalles.Add(new DespachoDetalle
                {
                    DespachoId = despachoId,
                    InventarioRepuestoId = item.InventarioRepuestoId,
                    ServicioId = item.ServicioId,
                    Cantidad = item.Cantidad,
                    Valor = subtotal
                });

                total += subtotal;
            }

            return total;
        }

        // DEVOLVER REPUESTOS AL INVENTARIO
        private async Task DevolverInventario(
            List<DespachoDetalle> detalles)
        {
            foreach (var detalle in detalles)
            {
                if (!detalle.InventarioRepuestoId.HasValue)
                    continue;

                var repuesto = await _context.InventarioRepuestos
                    .FindAsync(detalle.InventarioRepuestoId.Value);

                if (repuesto == null)
                    throw new ArgumentException(
                        "No se encontró el repuesto para restaurar el inventario");

                repuesto.Cantidad += detalle.Cantidad;
            }
        }

        // DATOS PARA CREAR UN DESPACHO
        public class GuardarDespachoRequest
        {
            public int RegistroId { get; set; }
            public string? ObsRecom { get; set; }
            public List<DetalleRequest> Detalles { get; set; } = new();
        }

        // DATOS PARA EDITAR UN DESPACHO
        public class EditarDespachoRequest
        {
            public string? ObsRecom { get; set; }
            public List<DetalleRequest> Detalles { get; set; } = new();
        }

        // DATOS DE SERVICIOS Y REPUESTOS
        public class DetalleRequest
        {
            public int? InventarioRepuestoId { get; set; }
            public int? ServicioId { get; set; }
            public int Cantidad { get; set; }
        }
    }
}
