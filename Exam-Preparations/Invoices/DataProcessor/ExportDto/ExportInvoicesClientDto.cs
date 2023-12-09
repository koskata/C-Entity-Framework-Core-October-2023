using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoicesClientDto
    {
        [Required]
        [Range(1000000000, 1500000000)]
        [XmlElement("InvoiceNumber")]
        public int Number { get; set; }

        [Required]
        [XmlIgnore]
        public DateTime IssueDate { get; set; }

        [Required]
        [XmlElement("InvoiceAmount")]
        public double Amount { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }


        [Required]
        [XmlElement("Currency")]
        public string CurrencyType { get; set; }
    }
}