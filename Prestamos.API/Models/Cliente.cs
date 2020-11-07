using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Prestamos.API.Models {
    public class Cliente {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombres { get; set; }

        public decimal Balance { get; set; }
    }
}
