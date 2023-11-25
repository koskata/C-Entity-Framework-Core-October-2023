namespace Invoices.DataProcessor
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using Invoices.Data;
    using Invoices.DataProcessor.ExportDto;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;

    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var clients = context.Clients
                            .Where(x => x.Invoices.Any(i => i.IssueDate > date))
                            .Select(x => new ExportClientDto
                            {
                                InvoicesCount = x.Invoices.Count,
                                ClientName = x.Name,
                                NumberVat = x.NumberVat,
                                Invoices = x.Invoices
                                .OrderBy(x => x.IssueDate).ThenByDescending(x => x.DueDate)
                                .Select(i => new ExportInvoiceDto
                                {
                                    InvoiceNumber = i.Number,
                                    InvoiceAmount = (double)i.Amount,
                                    DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                    Currency = i.CurrencyType,

                                    IssueDate = i.IssueDate,
                                }).ToArray()
                            }).OrderByDescending(x => x.InvoicesCount).ThenBy(x => x.ClientName).ToArray();

            var serializer = new XmlSerializer(typeof(ExportClientDto[]), new XmlRootAttribute("Clients"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(sb);

            serializer.Serialize(stringWriter, clients, namespaces);
            return sb.ToString().TrimEnd();
        }



        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                            .Where(x => x.ProductsClients.Any(x => x.Client.Name.Length >= nameLength))
                            .Select(x => new ExportProductDto
                            {
                                Name = x.Name,
                                Price = (double)x.Price,
                                Category = x.CategoryType,
                                Clients = x.ProductsClients
                                .Where(x => x.Client.Name.Length >= nameLength)
                                .Select(p => new ExportClientsForProductsDto
                                {
                                    Name = p.Client.Name,
                                    NumberVat = p.Client.NumberVat
                                }).OrderBy(x => x.Name).ToArray()
                            }).OrderByDescending(x => x.Clients.Count()).ThenBy(x => x.Name).Take(5).ToArray();

            string result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }
    }
}