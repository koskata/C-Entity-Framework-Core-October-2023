namespace Invoices.DataProcessor
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using AutoMapper.Execution;

    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ExportDto;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;

    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            

            var clients = context.Clients
                            .Where(x => x.Invoices.Any(p => p.IssueDate > date))
                            .ToArray()
                            .Select(x => new ExportClientDto
                            {
                                Count = x.Invoices.Count,
                                Name = x.Name,
                                NumberVat = x.NumberVat,

                                Invoices = x.Invoices
                                .OrderBy(x => x.IssueDate)
                                    .ThenByDescending(x => x.DueDate)
                                .Select(i => new ExportInvoicesClientDto
                                {
                                    Number = i.Number,
                                    IssueDate = i.IssueDate,
                                    Amount = (double)i.Amount,
                                    DueDate = i.DueDate.ToString("MM/dd/yyyy"),
                                    CurrencyType = i.CurrencyType.ToString()
                                }).ToArray()

                            }).OrderByDescending(x => x.Invoices.Length).ThenBy(x => x.Name).ToArray();

            var serializer = new XmlSerializer(typeof(ExportClientDto[]), new XmlRootAttribute("Clients"));

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, clients, namespaces);

            return sb.ToString().Trim();
        }



        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                            .Where(x => x.ProductsClients.Any(p => p.Client.Name.Length >= nameLength))
                            .Select(x => new ExportProductDto
                            {
                                Name = x.Name,
                                Price = (double)x.Price,
                                Category = x.CategoryType.ToString(),
                                Clients = x.ProductsClients
                                .Where(x => x.Client.ProductsClients.Any(p => p.Client.Name.Length >= nameLength))
                                .Select(pc => new ExportClientsProductDto
                                {
                                    Name = pc.Client.Name,
                                    NumberVat = pc.Client.NumberVat
                                }).OrderBy(x => x.Name).ToArray()
                            })
                            .OrderByDescending(x => x.Clients.Count())
                            .ThenBy(x => x.Name)
                            .Take(5).ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}