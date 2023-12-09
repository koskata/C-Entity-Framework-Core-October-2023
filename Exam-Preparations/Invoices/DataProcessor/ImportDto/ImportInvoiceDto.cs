﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [Required]
        [Range(1000000000, 1500000000)]
        public int Number { get; set; }

        [Required]
        public string IssueDate { get; set; }

        [Required]
        public string DueDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int CurrencyType { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}
