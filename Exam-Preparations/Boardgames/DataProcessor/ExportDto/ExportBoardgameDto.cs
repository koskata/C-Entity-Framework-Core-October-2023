using Boardgames.Data.Models.Enums;

namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportBoardgameDto
    {
        public string Name { get; set; }

        public double Rating { get; set; }

        public string Mechanics { get; set; }

        public CategoryType Category { get; set; }
    }
}