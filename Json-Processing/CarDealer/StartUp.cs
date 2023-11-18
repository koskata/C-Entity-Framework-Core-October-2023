using System.Diagnostics;

using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;

using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();


            // 9.
            //string suppliersInputJson = File.ReadAllText("../../../Datasets/suppliers.json");

            // 10.
            //string partsInputJson = File.ReadAllText("../../../Datasets/parts.json");

            // 11.
            //string carsInputJson = File.ReadAllText("../../../Datasets/cars.json");

            // 12. 
            //string customersInputJson = File.ReadAllText("../../../Datasets/customers.json");

            // 13. 
            //string salesInputJson = File.ReadAllText("../../../Datasets/sales.json");


            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {

            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson).ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }


        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson).Where(x => x.SupplierId <= 31).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<CarDTO>>(inputJson);

            foreach (var car in cars)
            {
                Car currentCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TraveledDistance = car.TraveledDistance
                };

                foreach (var part in car.PartsId)
                {
                    bool isValid = currentCar.PartsCars.FirstOrDefault(x => x.PartId == part) == null;
                    bool isPartValid = context.Parts.FirstOrDefault(p => p.Id == part) != null;

                    if (isValid && isPartValid)
                    {
                        currentCar.PartsCars.Add(new PartCar()
                        {
                            PartId = part
                        });
                    }
                }

                context.Cars.Add(currentCar);
            }

            context.SaveChanges();

            return $"Successfully imported {context.Cars.Count()}.";
        }


        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.AddRange(customers);
            context.SaveChanges();


            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                            .OrderBy(x => x.BirthDate).ThenBy(x => x.IsYoungDriver)
                            .Select(x => new
                            {
                                Name = x.Name,
                                BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                                x.IsYoungDriver
                            }).ToList();

            string json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                                .Select(x => new
                                {
                                    Id = x.Id,
                                    Make = x.Make,
                                    Model = x.Model,
                                    TraveledDistance = x.TraveledDistance
                                }).Where(x => x.Make == "Toyota")
                                .OrderBy(x => x.Model).ThenByDescending(x => x.TraveledDistance).ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                            .Where(x => x.IsImporter == false)
                            .Select(x => new
                            {
                                x.Id,
                                x.Name,
                                PartsCount = x.Parts.Count
                            }).ToList();


            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                        .Select(x => new
                        {
                            car = new
                            {
                                Make = x.Make,
                                Model = x.Model,
                                TraveledDistance = x.TraveledDistance
                            },

                            parts = x.PartsCars.Select(pc => new
                            {
                                Name = pc.Part.Name,
                                Price = pc.Part.Price.ToString("f2")
                            }).ToList()
                        }).ToList();



            return JsonConvert.SerializeObject(cars, Formatting.Indented);

        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                                .Select(x => new
                                {
                                    fullName = x.Name,
                                    boughtCars = x.Sales.Count,
                                    spentMoney = x.Sales.SelectMany(x => x.Car.PartsCars).Sum(x => x.Part.Price)
                                }).Where(x => x.boughtCars >= 1)
                                .OrderByDescending(x => x.spentMoney).ThenByDescending(x => x.boughtCars).ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                            .Select(x => new
                            {
                                car = new
                                {
                                    x.Car.Make,
                                    x.Car.Model,
                                    x.Car.TraveledDistance
                                },

                                customerName = x.Customer.Name,
                                discount = x.Discount.ToString("f2"),
                                price = x.Car.PartsCars.Sum(p => p.Part.Price).ToString("f2"),
                                priceWithDiscount = (x.Car.PartsCars.Sum(pc => pc.Part.Price)
                                                        - ((x.Car.PartsCars.Sum(pc => pc.Part.Price) * x.Discount) / 100)).ToString("f2")
                            }).Take(10).ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}