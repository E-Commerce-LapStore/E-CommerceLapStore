using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string Street { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Governorate { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        [ForeignKey("user")]
        [Required]
        public int UserId { get; set; }
        
        // Navigation Properties
        public virtual User user { get; set; }

    }
}
