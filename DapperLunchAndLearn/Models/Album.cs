using AO.Models;
using AO.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DapperLunchAndLearn.Models
{
    public class Album : IModel<int>
    {
        public int Id { get; set; }

        [Key]
        [References(typeof(Artist))]
        public int ArtistId { get; set; }

        [Key]
        [MaxLength(100)]
        public string Title { get; set; }

        public int? Year { get; set; }
    }
}
