using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

using Medicines.Data.Models.Enums;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Medicine")]
    public class ExportManyMedicinesDto
    {
        [Required]
        [XmlAttribute]
        public string Category { get; set; }


        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        [XmlElement]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 1000.00)]
        [XmlElement]
        public string Price { get; set; }



        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        [XmlElement]
        public string Producer { get; set; }

        [Required]
        [XmlElement("BestBefore")]
        public string ExpiryDate { get; set; }

    }
}