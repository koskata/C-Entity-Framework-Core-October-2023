using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Invoices.Data.Models;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientDto
    {
        [Required]
        [MinLength(10), MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MinLength(10), MaxLength(15)]
        public string NumberVat { get; set; }

        [XmlArray("Addresses")]
        public ImportAddressDto[] Addresses { get; set; }
    }
}
