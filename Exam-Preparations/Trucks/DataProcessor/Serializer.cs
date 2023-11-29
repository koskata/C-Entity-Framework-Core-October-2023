namespace Trucks.DataProcessor
{
    using System.Text;
    using System.Xml.Serialization;

    using Data;

    using Newtonsoft.Json;

    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers = context.Despatchers
                                .Where(x => x.Trucks.Any())
                                .Select(x => new ExportDespatchersDto
                                {
                                    Count = x.Trucks.Count,
                                    Name = x.Name,
                                    Trucks = x.Trucks.Select(t => new ExportTrucksDto
                                    {
                                        RegistrationNumber = t.RegistrationNumber,
                                        Make = t.MakeType.ToString()
                                    }).OrderBy(x => x.RegistrationNumber).ToArray()
                                }).OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToArray();


            var serializer = new XmlSerializer(typeof(ExportDespatchersDto[]), new XmlRootAttribute("Despatchers"));

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, despatchers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                            .Where(x => x.ClientsTrucks.Any(x => x.Truck.TankCapacity >= capacity))
                            .Select(x => new
                            {
                                Name = x.Name,
                                Trucks = x.ClientsTrucks
                                .Where(ct => ct.Truck.TankCapacity >= capacity)
                                .Select(c => new
                                {
                                    TruckRegistrationNumber = c.Truck.RegistrationNumber,
                                    VinNumber = c.Truck.VinNumber,
                                    TankCapacity = c.Truck.TankCapacity,
                                    CargoCapacity = c.Truck.CargoCapacity,
                                    CategoryType = c.Truck.CategoryType,
                                    MakeType = c.Truck.MakeType,
                                }).OrderBy(x => x.MakeType).ThenByDescending(x => x.CargoCapacity).ToArray()
                            }).OrderByDescending(x => x.Trucks.Count()).ThenBy(x => x.Name).Take(10).ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }
    }
}
