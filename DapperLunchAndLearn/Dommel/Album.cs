using AO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Dommel
{
    [Table("Album")] // table attribute works around Dommell's auto-pluralize behavior
    public class Album 
    {
        [Key]
        public int Id { get; set; }

        [References(typeof(Artist), CascadeDelete = true)]
        public int ArtistId { get; set; }
        
        [MaxLength(100)]
        public string Title { get; set; }

        public int? Year { get; set; }

        public string CreatedBy { get; set; }

        public IEnumerable<Song> Songs { get; set; }

        [NotMapped]
        public string ArtistName { get; set; }
    }
}
