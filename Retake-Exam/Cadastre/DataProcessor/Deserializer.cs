namespace Cadastre.DataProcessor
{
    using Cadastre.Data;
    using Cadastre.Data.Enumerations;
    using Cadastre.Data.Models;
    using Cadastre.DataProcessor.ImportDtos;

    using Newtonsoft.Json;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {
            var serializer = new XmlSerializer(typeof(ImportDistrictDto[]), new XmlRootAttribute("Districts"));

            var districtsDto = (ImportDistrictDto[])serializer.Deserialize(new StringReader(xmlDocument));

            List<District> districts = new List<District>();

            StringBuilder sb = new StringBuilder();

            foreach (var districtDto in districtsDto)
            {
                if (!IsValid(districtDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (dbContext.Districts.Any(x => x.Name == districtDto.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var districtToAdd = new District()
                {
                    Region = districtDto.Region,
                    Name = districtDto.Name,
                    PostalCode = districtDto.PostalCode,
                };

                foreach (var propertyDto in districtDto.Properties)
                {
                    if (!IsValid(propertyDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (propertyDto.Area < 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime dateOfAcquisition = DateTime.ParseExact(propertyDto.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture);


                    if (dbContext.Properties.Any(x => x.PropertyIdentifier == propertyDto.PropertyIdentifier)
                            || districtToAdd.Properties.Any(x => x.PropertyIdentifier == propertyDto.PropertyIdentifier))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (dbContext.Properties.Any(x => x.Address == propertyDto.Address)
                            || districtToAdd.Properties.Any(x => x.Address == propertyDto.Address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var propertyToAdd = new Property()
                    {
                        PropertyIdentifier = propertyDto.PropertyIdentifier,
                        Area = propertyDto.Area,
                        Details = propertyDto.Details,
                        Address = propertyDto.Address,
                        DateOfAcquisition = dateOfAcquisition,
                    };

                    districtToAdd.Properties.Add(propertyToAdd);

                }

                districts.Add(districtToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedDistrict, districtToAdd.Name, districtToAdd.Properties.Count));
            }

            dbContext.Districts.AddRange(districts);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {


            //•	Area – int not negative(required)

            var citizensDto = JsonConvert.DeserializeObject<ImportCitizenDto[]>(jsonDocument);

            List<Citizen> citizens = new List<Citizen>();

            StringBuilder sb = new StringBuilder();

            foreach (var citizenDto in citizensDto)
            {
                if (!IsValid(citizenDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (citizenDto.MaritalStatus != "Unmarried" && citizenDto.MaritalStatus != "Married"
                        && citizenDto.MaritalStatus != "Divorced" && citizenDto.MaritalStatus != "Widowed")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime birth = DateTime.ParseExact(citizenDto.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var citizenToAdd = new Citizen()
                {
                    FirstName = citizenDto.FirstName,
                    LastName = citizenDto.LastName,
                    BirthDate = birth,
                    MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus), citizenDto.MaritalStatus),
                };

                foreach (var id in citizenDto.Properties.Distinct())
                {
                    if (citizenToAdd.PropertiesCitizens.Any(x => x.PropertyId == id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    citizenToAdd.PropertiesCitizens.Add(new PropertyCitizen()
                    {
                        PropertyId = id,
                    });
                }

                citizens.Add(citizenToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedCitizen, citizenToAdd.FirstName, citizenToAdd.LastName, citizenToAdd.PropertiesCitizens.Count));
            }

            dbContext.Citizens.AddRange(citizens);
            dbContext.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
