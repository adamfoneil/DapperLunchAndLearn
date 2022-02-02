using AO.Models;
using DapperLunchAndLearn.Models.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{
    [Table("Album")] // table attribute works around Dommell's auto-pluralize behavior
    public class Album : BaseTable
    {
        [Key]
        [References(typeof(Artist))]
        public int ArtistId { get; set; }

        [Key]
        [MaxLength(100)]
        public string Title { get; set; }

        public int? Year { get; set; }

        public IEnumerable<Song> Songs { get; set; }
    }
}
