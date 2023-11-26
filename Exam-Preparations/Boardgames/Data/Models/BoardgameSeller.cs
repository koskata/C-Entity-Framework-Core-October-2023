using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class BoardgameSeller
    {
        public int BoardgameId { get; set; }
        [ForeignKey(nameof(BoardgameId))]
        public Boardgame Boardgame { get; set; }
        public int SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set;}
    }

    //•	BoardgameId – integer, Primary Key, foreign key (required)
    //•	Boardgame – Boardgame
    //•	SellerId – integer, Primary Key, foreign key(required)
    //•	Seller – Seller

}
