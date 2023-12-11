using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.DataProcessor.ExportDtos
{
    public class ExportPropertyDto
    {
        [Required]
        [MinLength(16)]
        [MaxLength(20)]
        public string PropertyIdentifier { get; set; }

        [Required]
        public int Area { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public string DateOfAcquisition { get; set; }

        public ExportOwnersDto[] Owners { get; set; }
    }
}
