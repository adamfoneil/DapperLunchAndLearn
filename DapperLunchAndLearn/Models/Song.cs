using AO.Models;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{
    public class Song : IModel<int>
    {
        public int Id { get; set; }
        
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
