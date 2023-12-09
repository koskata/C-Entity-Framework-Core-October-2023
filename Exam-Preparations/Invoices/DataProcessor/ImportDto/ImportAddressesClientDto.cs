using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class ImportAddressesClientDto
    {
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        [XmlElement]
        public string StreetName { get; set; }

        [Required]
        [XmlElement]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement]
        public string PostCode { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [XmlElement]
        public string City { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [XmlElement]
        public string Country { get; set; }
    }
}