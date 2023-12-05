using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Medicines.Data.Models.Enums;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [Required]
        [XmlAttribute("category")]
        public int Category { get; set; }

        [XmlElement]
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; }

        [XmlElement]
        [Required]
        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }


        [XmlElement]
        [Required]
        public string ProductionDate { get; set; }

        [XmlElement]
        [Required]
        public string ExpiryDate { get; set; }

        [XmlElement]
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; }
    }
}
