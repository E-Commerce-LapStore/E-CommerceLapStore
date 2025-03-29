using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Review
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("user")]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }

        public string? ReviewText { get; set; }

        public DateTime ReviewDate { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual Product product { get; set; }
    }
}
