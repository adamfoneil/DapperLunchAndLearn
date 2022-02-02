using DapperLunchAndLearn.Models.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{
    [Table("Artist")] // table attribute works around Dommell's auto-pluralize behavior
    public class Artist : BaseTable
    {        
        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        public IEnumerable<Album> Albums { get; set; }
    }
}
