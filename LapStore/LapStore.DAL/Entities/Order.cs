using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public enum OrderStatus { None, Pending, Processing, Shipped, Delivered, Cancelled }
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public OrderStatus Status { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [ForeignKey("user")]
        [Required]
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual ICollection<OrderItem>? orderItems { get; set; }

    }
}
