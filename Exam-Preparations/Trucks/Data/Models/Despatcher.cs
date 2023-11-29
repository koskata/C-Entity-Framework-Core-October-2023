using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Trucks.Data.Models
{
    public class Despatcher
    {
        public Despatcher()
        {
            Trucks = new HashSet<Truck>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } = null;

        [Required]
        public string Position { get; set; }

        public ICollection<Truck> Trucks { get; set; } = null!;
    }

        //•	Id – integer, Primary Key
        //•	Name – text with length[2, 40] (required)
        //•	Position – text
        //•	Trucks – collection of type Truck

}
