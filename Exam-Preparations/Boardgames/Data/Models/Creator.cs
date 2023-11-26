using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class Creator
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2), MaxLength(7)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2), MaxLength(7)]
        public string LastName { get; set; }

        public ICollection<Boardgame> Boardgames { get; set; } = new HashSet<Boardgame>();
    }
}
