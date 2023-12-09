using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportClientsProductDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        public string NumberVat { get; set; }
    }
}