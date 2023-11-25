namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Diagnostics.Metrics;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using AutoMapper.Execution;

    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;

    using Microsoft.VisualBasic;

    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";



        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportClientDto[]), new XmlRootAttribute("Clients"));

            var clientsDto = (ImportClientDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            List<Client> clients = new List<Client>();

            foreach (var client in clientsDto)
            {
                if (!IsValid(client))
                {
                    sb.AppendLine(ErrorMessage);
                }

                else
                {
                    var clientToAdd = new Client()
                    {
                        Name = client.Name,
                        NumberVat = client.NumberVat,
                    };


                    foreach (var address in client.Addresses)
                    {
                        if (!IsValid(address))
                        {
                            sb.AppendLine(ErrorMessage);
                        }

                        else
                        {
                            var addressToAdd = new Address()
                            {
                                StreetName = address.StreetName,
                                StreetNumber = address.StreetNumber,
                                PostCode = address.PostCode,
                                City = address.City,
                                Country = address.Country
                            };

                            clientToAdd.Addresses.Add(addressToAdd);
                        }

                    }


                    clients.Add(clientToAdd);
                    sb.AppendLine(String.Format(SuccessfullyImportedClients, client.Name));
                }

            }

            context.Clients.AddRange(clients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            var invoicesDto = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString).ToList();

            StringBuilder sb = new StringBuilder();

            List<Invoice> invoices = new List<Invoice>();

            foreach (var invoiceDto in invoicesDto)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (invoiceDto.DueDate == DateTime.ParseExact
                    ("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture) ||
                    invoiceDto.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var invoiceToAdd = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = invoiceDto.IssueDate,
                    DueDate = invoiceDto.DueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                if (invoiceToAdd.IssueDate > invoiceToAdd.DueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                

                invoices.Add(invoiceToAdd);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoiceToAdd.Number));
            }

            context.Invoices.AddRange(invoices);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            var productsDto = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString).ToList();

            var sb = new StringBuilder();

            List<Product> products = new List<Product>();

            //List<Address> addresses = new List<Address>();

            List<int> clientIds = context.Clients.Select(x => x.Id).ToList();

            foreach (var productDto in productsDto)
            {
                if (!IsValid(productDto))
                {
                    sb.AppendLine(ErrorMessage);
                }

                else
                {
                    var productToAdd = new Product()
                    {
                        Name = productDto.Name,
                        Price = productDto.Price,
                        CategoryType = productDto.CategoryType,
                    };

                    foreach (var id in productDto.Clients.Distinct())
                    {
                        if (!clientIds.Contains(id))
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                        else
                        {
                            productToAdd.ProductsClients.Add(new ProductClient
                            {
                                ClientId = id,
                            });
                        }
                    }

                    products.Add(productToAdd);
                    sb.AppendLine(String.Format(SuccessfullyImportedProducts, productToAdd.Name, productToAdd.ProductsClients.Count));
                }
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
