using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Boardgame")]
    public class ExportBoardgamesCreatorDto
    {
        [XmlElement("BoardgameName")]
        public string Name { get; set; }

        [XmlElement("BoardgameYearPublished")]
        public int Year { get; set; }
    }
}