using AO.Models;
using DapperLunchAndLearn.Models.Conventions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{
    [Table(nameof(Song))]
    public class Song : BaseTable
    {
        [References(typeof(Album), CascadeDelete = true)]
        public int AlbumId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        public int? TrackNumber { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? Duration { get; set; }
    }
}
