using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [ForeignKey(nameof(SongId))]
        public int SongId { get; set; }
        public Song Song { get; set; }

        [ForeignKey(nameof(PerformerId))]
        public int PerformerId { get; set; }
        public Performer Performer { get; set; }
    }
}
