using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invoices.Data.Models.Enums;

using static System.Net.Mime.MediaTypeNames;

namespace Invoices.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(9), MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        public ICollection<ProductClient> ProductsClients { get; set; } = new HashSet<ProductClient>();
    }

    //•	Id – integer, Primary Key
    //•	Name – text with length[9…30] (required)
    //•	Price – decimal in range[5.00…1000.00] (required)
    //•	CategoryType – enumeration of type CategoryType, with possible values(ADR, Filters, Lights, Others, Tyres) (required)
    //•	ProductsClients – collection of type ProductClient

}
