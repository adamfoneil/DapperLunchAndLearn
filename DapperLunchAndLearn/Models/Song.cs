using AO.Models;
using DapperLunchAndLearn.Models.Conventions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{
    [Table("Song")] // table attribute works around Dommell's auto-pluralize behavior
    public class Song : BaseTable
    {
        [References(typeof(Album))]
        public int AlbumId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        public int? TrackNumber { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? Duration { get; set; }
    }
}
