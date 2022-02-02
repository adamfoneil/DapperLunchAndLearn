using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DapperLunchAndLearn.Dommel
{
    [Table("Artist")] // table attribute works around Dommell's auto-pluralize behavior
    public class Artist 
    {        
        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string Name { get; set; }

        public string CreatedBy { get; set; }

        public IEnumerable<Album> Albums { get; set; }
    }
}
