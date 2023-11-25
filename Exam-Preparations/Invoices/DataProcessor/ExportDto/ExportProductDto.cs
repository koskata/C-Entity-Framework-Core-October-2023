using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportProductDto
    {
        public string Name { get; set; }

        public double Price { get; set; }

        [EnumDataType(typeof(CategoryType))]
        public CategoryType Category { get; set; }

        public ExportClientsForProductsDto[] Clients { get; set; }
    }
}
