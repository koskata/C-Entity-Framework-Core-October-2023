namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Metrics;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;

    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportCreatorDto[]), new XmlRootAttribute("Creators"));

            var creatorsDto = (ImportCreatorDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();

            List<Creator> creators = new List<Creator>();

            foreach (var creatorDto in creatorsDto)
            {
                if (!IsValid(creatorDto))
                {
                    sb.AppendLine(ErrorMessage);
                }
                else
                {
                    var creatorToAdd = new Creator()
                    {
                        FirstName = creatorDto.FirstName,
                        LastName = creatorDto.LastName,
                    };

                    foreach (var boardgame in creatorDto.Boardgames)
                    {
                        if (!IsValid(boardgame))
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                        else
                        {
                            var boardgameToAdd = new Boardgame()
                            {
                                Name = boardgame.Name,
                                Rating = boardgame.Rating,
                                YearPublished = boardgame.YearPublished,
                                CategoryType = (CategoryType)boardgame.CategoryType,
                                Mechanics = boardgame.Mechanics
                            };

                            creatorToAdd.Boardgames.Add(boardgameToAdd);
                        }
                    }

                    creators.Add(creatorToAdd);
                    sb.AppendLine(String.Format(SuccessfullyImportedCreator, creatorToAdd.FirstName, creatorToAdd.LastName, creatorToAdd.Boardgames.Count));
                }
            }

            context.Creators.AddRange(creators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }




        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sellersDto = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Seller> sellers = new List<Seller>();

            List<int> boardgamesIds = context.Boardgames.Select(x => x.Id).ToList();

            foreach (var sellerDto in sellersDto)
            {
                if (!IsValid(sellerDto))
                {
                    sb.AppendLine(ErrorMessage);
                }
                else
                {
                    var sellerToAdd = new Seller()
                    {
                        Name = sellerDto.Name,
                        Address = sellerDto.Address,
                        Country = sellerDto.Country,
                        Website = sellerDto.Website,
                    };

                    foreach (var id in sellerDto.Boardgames.Distinct())
                    {
                        if (!boardgamesIds.Contains(id))
                        {
                            sb.AppendLine(ErrorMessage);
                        }
                        else
                        {
                            sellerToAdd.BoardgamesSellers.Add(new BoardgameSeller
                            {
                                BoardgameId = id
                            });
                        }

                    }

                    sellers.Add(sellerToAdd);
                    sb.AppendLine(String.Format(SuccessfullyImportedSeller, sellerToAdd.Name, sellerToAdd.BoardgamesSellers.Count));
                }
            }

            context.Sellers.AddRange(sellers);
            context.SaveChanges();

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
