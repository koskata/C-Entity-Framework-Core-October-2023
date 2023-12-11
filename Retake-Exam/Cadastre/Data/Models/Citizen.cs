using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cadastre.Data.Enumerations;

using static System.Net.Mime.MediaTypeNames;

namespace Cadastre.Data.Models
{
    public class Citizen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public MaritalStatus MaritalStatus { get; set; }

        public ICollection<PropertyCitizen> PropertiesCitizens { get; set; } = new HashSet<PropertyCitizen>();
    }

        //•	Id – integer, Primary Key
        //•	FirstName – text with length[2, 30] (required)
        //•	LastName – text with length[2, 30] (required)
        //•	BirthDate – DateTime(required)
        //•	MaritalStatus - MaritalStatus enum (Unmarried = 0, Married, Divorced, Widowed) (required)
        //•	PropertiesCitizens - collection of type PropertyCitizen

}
