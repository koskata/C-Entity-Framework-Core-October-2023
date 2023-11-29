using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Trucks.Data.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MakeType
    {
        Daf,
        Man, 
        Mercedes,
        Scania, 
        Volvo
    }
}
