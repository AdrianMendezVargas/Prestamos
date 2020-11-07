using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Prestamos.API.Models {
    public class Mora {
        [Key]
        public int MoraId { get; set; }

        [Required]
        public int PrestamoId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public Mora() {
            Monto = 0m;
            Fecha = DateTime.Now;
        }
    }
}
