using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Entities
{
    [Index("ZipCode", "City", "Governorate", "Country")]
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please enter the post code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter the governorate")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "Please enter the city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the country")]
        public string Country { get; set; }
         

        // Navigation Properties
        public virtual ICollection<User> users { get; set; }

    }
}
