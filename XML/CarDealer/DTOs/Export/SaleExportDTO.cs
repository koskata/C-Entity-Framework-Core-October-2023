﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class SaleExportDTO
    {
        [XmlElement("car")]
        public CarExportForLastExcDTO Car { get; set; }

        //[XmlElement("car")]
        //public string Make { get; set; }
        //public string Model { get; set; }
        //public string TraveledDistance { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set;}
    }
}
