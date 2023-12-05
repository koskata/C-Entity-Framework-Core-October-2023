using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Medicines.Data.Models.Enums;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public int AgeGroup { get; set; }

        [Required]
        public int Gender { get; set; }

        public int[] Medicines { get; set; }
    }
}
