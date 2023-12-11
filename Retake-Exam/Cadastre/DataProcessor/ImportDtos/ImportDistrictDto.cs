using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Cadastre.Data.Enumerations;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType("District")]
    public class ImportDistrictDto
    {
        [Required]
        [XmlAttribute]
        public Region Region { get; set; }


        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        [XmlElement]
        public string Name { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression(@"^[A-Z]{2}-[0-9]{5}$")]
        [XmlElement]
        public string PostalCode { get; set; }

        [XmlArray("Properties")]
        public ImportPropertiesDto[] Properties { get; set; }
    }
}
