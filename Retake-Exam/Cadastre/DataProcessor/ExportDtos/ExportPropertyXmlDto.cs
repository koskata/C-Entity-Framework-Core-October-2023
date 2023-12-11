using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ExportDtos
{
    [XmlType("Property")]
    public class ExportPropertyXmlDto
    {
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression(@"^[A-Z]{2}-[0-9]{5}$")]
        [XmlAttribute("postal-code")]
        public string PostalCode { get; set; }

        [Required]
        [MinLength(16)]
        [MaxLength(20)]
        [XmlElement]
        public string PropertyIdentifier { get; set; }

        [Required]
        [XmlElement]
        public int Area { get; set; }

        [Required]
        [XmlElement]
        public string DateOfAcquisition { get; set; }
    }
}
