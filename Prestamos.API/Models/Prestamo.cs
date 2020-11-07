using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Prestamos.API.Models {
    public class Prestamo {

        [Key]
        public int PrestamoId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public DateTime Fecha { get; set; }

        public string Concepto { get; set; }

        [Required]
        public decimal Monto { get; set; }

        public decimal Balance { get; set; }

        [ForeignKey("PrestamoId")]
        public virtual List<Mora> Moras { get; set; }

        public virtual decimal BalanceMoras() {
            decimal balanceMoras = 0m;
            foreach (var mora in Moras) {
                balanceMoras += mora.Monto;
            }
            return balanceMoras;
        }



        public Prestamo() {
            Monto = 0m;
            Balance = 0m;
            Fecha = DateTime.Now;
            Moras = new List<Mora>();
        }

    }
}
