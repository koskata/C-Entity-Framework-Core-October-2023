using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoiceDto
    {
        [XmlElement("InvoiceNumber")]
        public int InvoiceNumber { get; set; }

        [XmlElement("InvoiceAmount")]
        public double InvoiceAmount { get; set; }

        [XmlElement("DueDate")]
        public string? DueDate { get; set; }

        [XmlElement("Currency")]
        [EnumDataType(typeof(CurrencyType))]
        public CurrencyType Currency { get; set; }


        [XmlIgnore]
        public DateTime IssueDate { get; set; }
    }
}
