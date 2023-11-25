using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Invoices.Data.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryType
    {
        ADR, 
        Filters, 
        Lights, 
        Others, 
        Tyres
    }
}
