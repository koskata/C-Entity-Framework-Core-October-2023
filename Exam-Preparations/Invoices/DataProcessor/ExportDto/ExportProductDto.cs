using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportProductDto
    {
        [Required]
        [MinLength(9)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Range(5.00, 1000.00)]
        public double Price { get; set; }

        [Required]
        public string Category { get; set; }

        public ExportClientsProductDto[] Clients { get; set; }
    }
}
