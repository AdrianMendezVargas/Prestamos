using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prestamos.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prestamos.API.Data {
    public class Contexto : DbContext{

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        public Contexto(DbContextOptions options) : base(options) {}

    }
}
