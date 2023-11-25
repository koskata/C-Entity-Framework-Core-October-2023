using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invoices.Data.Models.Enums;

using Invoices.Data.Models;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [Required]
        [MinLength(9), MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }

        [Required]
        [EnumDataType(typeof(CategoryType))]
        public CategoryType CategoryType { get; set; }

        public int[] Clients { get; set; }
    }
}
