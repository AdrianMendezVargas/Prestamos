using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prestamos.API.Data;
using Prestamos.API.Models;

namespace Prestamos.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase {
        private readonly Contexto _context;

        public ClientesController(Contexto context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes() {
            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id) {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null) {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> SaveCliente(Cliente cliente) {
            if (ClienteExists(cliente.Id)) {
                return await PutCliente(cliente);
            } else {
                return await PostCliente(cliente);
            }
        }

        public async Task<ActionResult<Cliente>> PutCliente(Cliente cliente) {

            _context.Entry(cliente).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception) {

                throw;

            }

            return CreatedAtAction("GetCliente" , new { id = cliente.Id } , cliente);
        }

        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente) {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente" , new { id = cliente.Id } , cliente);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cliente>> DeleteCliente(int id) {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        private bool ClienteExists(int id) {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
