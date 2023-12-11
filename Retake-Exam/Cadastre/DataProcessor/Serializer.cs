using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Serialization;

using Cadastre.Data;
using Cadastre.Data.Enumerations;
using Cadastre.DataProcessor.ExportDtos;

using Newtonsoft.Json;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {
            var properties = dbContext.Properties
                             .Where(x => x.DateOfAcquisition >= DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                             .ToArray()
                             .Select(x => new ExportPropertyDto
                             {
                                 PropertyIdentifier = x.PropertyIdentifier,
                                 Area = x.Area,
                                 Address = x.Address,
                                 DateOfAcquisition = x.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                 Owners = x.PropertiesCitizens
                                 .Where(x => x.Property.DateOfAcquisition >= DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                                 .ToArray()
                                 .Select(pc => new ExportOwnersDto
                                 {
                                     LastName = pc.Citizen.LastName,
                                     MaritalStatus = pc.Citizen.MaritalStatus.ToString()
                                 }).OrderBy(x => x.LastName).ToArray()
                             }).OrderByDescending(x => DateTime.ParseExact(x.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ThenBy(x => x.PropertyIdentifier).ToArray();

            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }



        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            var properties = dbContext.Properties
                                .Where(x => x.Area >= 100)
                                .ToArray()
                                .Select(x => new ExportPropertyXmlDto
                                {
                                    PostalCode = x.District.PostalCode,
                                    PropertyIdentifier = x.PropertyIdentifier,
                                    Area = x.Area,
                                    DateOfAcquisition = x.DateOfAcquisition.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                                }).OrderByDescending(x => x.Area).ThenBy(x => DateTime.ParseExact(x.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToArray();

            var serializer = new XmlSerializer(typeof(ExportPropertyXmlDto[]), new XmlRootAttribute("Properties"));

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, properties, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
