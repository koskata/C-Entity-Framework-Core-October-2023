using System.ComponentModel.DataAnnotations;

using Cadastre.Data.Enumerations;

namespace Cadastre.DataProcessor.ExportDtos
{
    public class ExportOwnersDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public string MaritalStatus { get; set; }
    }
}