using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Prestamos.API.Controllers.HttpClientHelpers;
using Prestamos.API.Data;
using Prestamos.API.Models;

namespace Prestamos.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase {
        private readonly Contexto context;
        private readonly RequestHelper helper;

        public PrestamosController(Contexto _context, RequestHelper _helper) {
            this.context = _context;
            helper = _helper;
        }

        [HttpGet]
        public async Task<List<Prestamo>> GetPrestamos() {
            return await context.Prestamos.Include(p => p.Moras).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id) {
            var prestamo = await context.Prestamos.Include(p => p.Moras)
                .Where(p => p.PrestamoId == id)
                .FirstOrDefaultAsync();

            if (prestamo == null) {
                return NotFound();
            }

            return prestamo;
        }

        [HttpPost]
        public async Task<ActionResult<Prestamo>> SavePrestamo(Prestamo prestamo) {
            if (PrestamoExists(prestamo.PrestamoId)) {
                return await PutPrestamo(prestamo);
            } else {
                return await PostPrestamo(prestamo);
            }
        }

        public async Task<ActionResult<Prestamo>> PutPrestamo(Prestamo prestamo) {
            await context.Database.ExecuteSqlRawAsync($"Delete from Mora Where PrestamoId = {prestamo.PrestamoId}");

            foreach (var mora in prestamo.Moras) {
                context.Entry(mora).State = EntityState.Added;
            }

            context.Entry(prestamo).State = EntityState.Modified;

            try {
                bool paso = await context.SaveChangesAsync() > 0;

                if(paso) {
                    await ActualizarBalanceCliente(prestamo);
                }

            } catch (Exception) {

                throw;

            }

            return CreatedAtAction("GetPrestamo" , new { id = prestamo.PrestamoId } , prestamo);
        }

        public async Task<ActionResult<Prestamo>> PostPrestamo(Prestamo prestamo) {
            Cliente cliente = await RequestHelper.GetAsync<Cliente>($"clientes/{prestamo.ClienteId}");

            if (cliente.Nombres != null) {
                prestamo.Balance = prestamo.Monto;
                prestamo.Fecha = DateTime.Now;

                context.Prestamos.Add(prestamo);
                bool paso = await context.SaveChangesAsync() > 0;

                if (paso) {
                    await ActualizarBalanceCliente(prestamo);
                }
                return CreatedAtAction("GetPrestamo" , new { id = prestamo.PrestamoId } , prestamo);

            } else {
                return BadRequest("El cliente no existe");
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Prestamo>> DeletePrestamo(int id) {
            var prestamo = await context.Prestamos.FindAsync(id);

            if (prestamo == null) {
                return NotFound();
            }

            context.Prestamos.Remove(prestamo);
            bool paso = await context.SaveChangesAsync() > 0;

            if (paso) {
                await ActualizarBalanceCliente(prestamo);
            }

            return prestamo;
        }

        private async Task ActualizarBalanceCliente(Prestamo _prestamo) {

            var cliente = await RequestHelper.GetAsync<Cliente>($"clientes/{_prestamo.ClienteId}");

            if (cliente.Nombres != null) {

                var prestamos = (await GetPrestamos()).Where(p => p.ClienteId == cliente.Id).ToList();
                cliente.Balance = 0;
                foreach (var prestamo in prestamos) {
                    cliente.Balance += prestamo.Balance;

                    foreach (var mora in prestamo.Moras) {
                        cliente.Balance += mora.Monto;
                    }
                }
                await RequestHelper.PostAsync("clientes" , cliente);
            }
        }

        private bool PrestamoExists(int id) {
            return context.Prestamos.Any(e => e.PrestamoId == id);
        }
    }
}
