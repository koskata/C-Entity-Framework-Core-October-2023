using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Medicines.Data.Models.Enums;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class ExportPatientDto
    {
        [Required]
        [XmlAttribute("Gender")]
        public string Gender { get; set; }


        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [Required]
        [XmlElement]
        public string AgeGroup { get; set; }

        [XmlArray]
        public ExportManyMedicinesDto[] Medicines { get; set; }
    }
}
