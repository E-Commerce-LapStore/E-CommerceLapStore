using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("user")]
        [Required]
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
    }
}
