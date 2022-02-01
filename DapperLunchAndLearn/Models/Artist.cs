using AO.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DapperLunchAndLearn.Models
{
    public class Artist : IModel<int>
    {        
        public int Id { get; set; }

        [Key]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
