namespace Boardgames.DataProcessor
{
    using System.Text;
    using System.Xml.Serialization;

    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;

    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creators = context.Creators
                            .Where(x => x.Boardgames.Any()).ToArray()
                            .Select(x => new ExportCreatorDto
                            {
                                Count = x.Boardgames.Count(),
                                Name = $"{x.FirstName} {x.LastName}",
                                Boardgames = x.Boardgames.Select(b => new ExportBoardgamesCreatorDto
                                {
                                    Name = b.Name,
                                    Year = b.YearPublished
                                }).OrderBy(x => x.Name).ToArray()
                            }).OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToArray();

            var serializer = new XmlSerializer(typeof(ExportCreatorDto[]), new XmlRootAttribute("Creators"));

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, creators, namespaces);
            return sb.ToString().TrimEnd();
        }




        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                            .Where(x => x.BoardgamesSellers.Any(x => x.Boardgame.YearPublished >= year && x.Boardgame.Rating <= rating))
                            .Select(x => new ExportSellerDto
                            {
                                Name = x.Name,
                                Website = x.Website,
                                Boardgames = x.BoardgamesSellers
                                .Where(x => x.Boardgame.YearPublished >= year && x.Boardgame.Rating <= rating)
                                .Select(b => new ExportBoardgameDto
                                {
                                    Name = b.Boardgame.Name,
                                    Rating = b.Boardgame.Rating,
                                    Mechanics = b.Boardgame.Mechanics,
                                    Category = b.Boardgame.CategoryType
                                }).OrderByDescending(x => x.Rating).ThenBy(x => x.Name).ToArray()
                            }).OrderByDescending(x => x.Boardgames.Count()).ThenBy(x => x.Name).Take(5).ToArray();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
        }
    }
}