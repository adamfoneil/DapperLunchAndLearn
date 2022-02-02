using AO.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Dommel
{
    [Table("Song")] // table attribute works around Dommell's auto-pluralize behavior
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [References(typeof(Album), CascadeDelete = true)]
        public int AlbumId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        public int? TrackNumber { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? Duration { get; set; }

        public string CreatedBy { get; set; }
    }
}
