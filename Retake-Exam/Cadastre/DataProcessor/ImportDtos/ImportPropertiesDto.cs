using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Cadastre.DataProcessor.ImportDtos
{
    [XmlType("Property")]
    public class ImportPropertiesDto
    {
        [Required]
        [MinLength(16)]
        [MaxLength(20)]
        [XmlElement]
        public string PropertyIdentifier { get; set; }

        [Required]
        [XmlElement]
        public int Area { get; set; }

        [MinLength(5)]
        [MaxLength(500)]
        [XmlElement]
        public string Details { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        [XmlElement]
        public string Address { get; set; }

        [Required]
        [XmlElement]
        public string DateOfAcquisition { get; set; }
    }
}