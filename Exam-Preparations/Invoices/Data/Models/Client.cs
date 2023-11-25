using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Invoices.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(10), MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MinLength(10), MaxLength(15)]
        public string NumberVat { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public ICollection<ProductClient> ProductsClients { get; set; } = new HashSet<ProductClient>();
    }

        //•	Id – integer, Primary Key
        //•	Name – text with length[10…25] (required)
        //•	NumberVat – text with length[10…15] (required)
        //•	Invoices – collection of type Invoicе
        //•	Addresses – collection of type Address
        //•	ProductsClients – collection of type ProductClient

}
