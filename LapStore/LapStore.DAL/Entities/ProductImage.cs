using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("product")]
        [Required]
        public int ProductId {  get; set; }

        public string Url { get; set; }

        // Navigation Properties
        public virtual Product product { get; set; }
    }
}
