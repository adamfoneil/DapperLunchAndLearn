using AO.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DapperLunchAndLearn.Models.Conventions
{
    public abstract class BaseTable : IModel<int>
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string CreatedBy { get; set; }
    }
}
