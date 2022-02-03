using DapperLunchAndLearn.Models.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Models
{    
    [Table(nameof(Artist))]
    public class Artist : BaseTable
    {        
        [MaxLength(50)]
        public string Name { get; set; }

        public IEnumerable<Album> Albums { get; set; }
    }
}
