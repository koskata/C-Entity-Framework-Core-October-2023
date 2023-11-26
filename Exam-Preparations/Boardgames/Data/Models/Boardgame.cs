using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Boardgames.Data.Models.Enums;

namespace Boardgames.Data.Models
{
    public class Boardgame
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(10), MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [Range(1.00, 10.00)]
        public double Rating { get; set; }

        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }

        [Required]
        [EnumDataType(typeof(CategoryType))]
        public CategoryType CategoryType { get; set; }

        [Required]
        public string Mechanics { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]

        public Creator Creator { get; set; }

        public ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new HashSet<BoardgameSeller>();
    }
}
