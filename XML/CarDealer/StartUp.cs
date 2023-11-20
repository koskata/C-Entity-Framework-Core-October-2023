using System.Text;
using System.Xml.Serialization;

using AutoMapper.QueryableExtensions;
using AutoMapper;

using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

using Castle.Core.Resource;

using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            // 1.
            //string suppliersInputXml = File.ReadAllText("../../../Datasets/suppliers.xml");

            // 2.
            //string partsInputXml = File.ReadAllText("../../../Datasets/parts.xml");

            // 3.
            //string carsInputXml = File.ReadAllText("../../../Datasets/cars.xml");

            // 4.
            //string customersInputXml = File.ReadAllText("../../../Datasets/customers.xml");

            // 5.
            //string salesInputXml = File.ReadAllText("../../../Datasets/sales.xml");

            Console.WriteLine(GetTotalSalesByCustomer(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(SupplierDTO[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (SupplierDTO[])serializer.Deserialize(new StringReader(inputXml));

            var suppliers = suppliersDto
                        .Select(x => new Supplier()
                        {
                            Name = x.Name,
                            IsImporter = x.IsImporter
                        }).ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(PartDTO[]), new XmlRootAttribute("Parts"));

            var partsDto = (PartDTO[])serializer.Deserialize(new StringReader(inputXml));

            var parts = partsDto
                    .Select(x => new Part()
                    {
                        Name = x.Name,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        SupplierId = x.SupplierId
                    }).Where(x => x.SupplierId >= 1 && x.SupplierId <= 31).ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CarDTO[]), new XmlRootAttribute("Cars"));

            var carsDto = (CarDTO[])serializer.Deserialize(new StringReader(inputXml));

            List<Car> cars = new List<Car>();

            int[] ids = context.Parts
                    .Select(x => x.Id)
                        .ToArray();

            foreach (var carDto in carsDto)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance,
                };

                int[] carPartIds = carDto.Parts
                    .Select(x => x.PartId)
                    .Distinct()
                    .ToArray();


                foreach (var id in carPartIds)
                {
                    if (ids.Contains(id))
                    {
                        car.PartsCars.Add(new PartCar
                        {
                            PartId = id
                        });

                    }

                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CustomerDTO[]), new XmlRootAttribute("Customers"));

            var customersDto = (CustomerDTO[])serializer.Deserialize(new StringReader(inputXml));

            var customers = customersDto
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                }).ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(SaleDTO[]), new XmlRootAttribute("Sales"));

            var salesDto = (SaleDTO[])serializer.Deserialize(new StringReader(inputXml));

            int[] ids = context.Cars.Select(x => x.Id).ToArray();

            var sales = salesDto
                    .Select(x => new Sale
                    {
                        CarId = x.CarId,
                        CustomerId = x.CustomerId,
                        Discount = x.Discount,
                    })
                    .Where(x => ids.Contains(x.CarId))
                    .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                        .Select(x => new CarExportDTO
                        {
                            Make = x.Make,
                            Model = x.Model,
                            TraveledDistance = x.TraveledDistance,
                        })
                        .Where(x => x.TraveledDistance > 2000000)
                        .OrderBy(x => x.Make).ThenBy(x => x.Model).Take(10).ToArray();

            var serializer = new XmlSerializer(typeof(CarExportDTO[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                        .Where(x => x.Make == "BMW")
                        .Select(x => new CarWithAttributesDTO
                        {
                            Id = x.Id,
                            Model = x.Model,
                            TraveledDistance = x.TraveledDistance,
                        })
                        .OrderBy(x => x.Model).ThenByDescending(x => x.TraveledDistance).ToArray();

            var serializer = new XmlSerializer(typeof(CarWithAttributesDTO[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, cars, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                            .Where(x => x.IsImporter == false)
                            .Select(x => new SupplierExportDTO
                            {
                                Id = x.Id,
                                Name = x.Name,
                                PartsCount = x.Parts.Count,
                            }).ToArray();

            var serializer = new XmlSerializer(typeof(SupplierExportDTO[]), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, suppliers, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                        .Select(x => new CarWithPartsExportDTO
                        {
                            Make = x.Make,
                            Model = x.Model,
                            TraveledDistance = x.TraveledDistance,
                            //Parts = (CarPartsExportDTO)x.PartsCars.Select(x => x.Part.Name)
                            Parts = x.PartsCars.Select(p => new CarPartsExportDTO
                            {
                                Name = p.Part.Name,
                                Price = p.Part.Price,
                            }).OrderByDescending(x => x.Price).ToArray(),
                        }).OrderByDescending(x => x.TraveledDistance).ThenBy(x => x.Model).Take(5).ToArray();

            var serializer = new XmlSerializer(typeof(CarWithPartsExportDTO[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, cars, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new CustomerExportDTO
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.IsYoungDriver
                                    ? c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => (decimal)Math.Round(pc.Part.Price * 0.95m, 2))
                                    : c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)

                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

        var serializer = new XmlSerializer(typeof(CustomerExportDTO[]), new XmlRootAttribute("customers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, customers, namespaces);
            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                    .Select(x => new SaleExportDTO
                    {
                        Car = new CarExportForLastExcDTO()
                        {
                            Make = x.Car.Make,
                            Model = x.Car.Model,
                            TraveledDistance = x.Car.TraveledDistance
                        },
                        Discount = x.Discount,
                        CustomerName = x.Customer.Name,
                        Price = x.Car.PartsCars.Sum(x => x.Part.Price),
                        PriceWithDiscount = x.Car.PartsCars.Sum(x => x.Part.Price) - x.Car.PartsCars.Sum(x => x.Part.Price)  * x.Discount / 100
                    }).ToArray();

            var serializer = new XmlSerializer(typeof(SaleExportDTO[]), new XmlRootAttribute("sales"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new(sb);

            serializer.Serialize(writer, sales, namespaces);
            return sb.ToString().TrimEnd();
        }
    }
}