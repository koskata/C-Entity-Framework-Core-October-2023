using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json.Linq;

using Trucks.Data.Models.Enums;

using static System.Net.Mime.MediaTypeNames;

namespace Trucks.Data.Models
{
    public class Truck
    {
        public Truck()
        {
            ClientsTrucks = new HashSet<ClientTruck>();
        }

        [Key]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Z]{2}\d{4}[A-Z]{2}")]
        [MinLength(8)]
        [MaxLength(8)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MinLength(17)]
        [MaxLength(17)]
        public string VinNumber { get; set; } = null!;

        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        public MakeType MakeType { get; set; }

        [Required]
        public int DespatcherId { get; set; }

        [ForeignKey(nameof(DespatcherId))]
        public Despatcher Despatcher { get; set; } = null!;

        public ICollection<ClientTruck> ClientsTrucks { get; set; } = null!;
    }

        //•	Id – integer, Primary Key
        //•	RegistrationNumber – text with length 8. First two characters are upper letters[A - Z], followed by four digits and the last two characters are upper letters[A - Z] again.
        //•	VinNumber – text with length 17 (required)
        //•	TankCapacity – integer in range[950…1420]
        //•	CargoCapacity – integer in range[5000…29000]
        //•	CategoryType – enumeration of type CategoryType, with possible values (Flatbed, Jumbo, Refrigerated, Semi) (required)
        //•	MakeType – enumeration of type MakeType, with possible values (Daf, Man, Mercedes, Scania, Volvo) (required)
        //•	DespatcherId – integer, foreign key (required)
        //•	Despatcher – Despatcher
        //•	ClientsTrucks – collection of type ClientTruck

}
