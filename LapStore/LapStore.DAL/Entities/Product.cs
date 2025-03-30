﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }


        [ForeignKey("category")]
        [Required]
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual Category category { get; set; }
        public virtual ICollection<CartItem>? cartItems { get; set; }
        public virtual ICollection<OrderItem>? orderItems { get; set; }
        public virtual ICollection<Review>? productReviews { get; set; }
        public virtual ICollection<ProductImage>? productImages { get; set; }
    }
}
