using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Trucks.Data.Models
{
    public class Client
    {
        public Client()
        {
            ClientsTrucks = new HashSet<ClientTruck>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        public ICollection<ClientTruck> ClientsTrucks { get; set; } = null!;
    }

        //•	Id – integer, Primary Key
        //•	Name – text with length[3, 40] (required)
        //•	Nationality – text with length[2, 40] (required)
        //•	Type – text(required)
        //•	ClientsTrucks – collection of type ClientTruck

}
