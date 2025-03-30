using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    public enum UserRole { None, Admin, Customer, Vendor }
    public enum UserGender { None, Male, Female }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store the hash, not the plain password

        public UserRole Role { get; set; }
        public UserGender Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateOnly BirthDate { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        // Navigation Properties
        public virtual ICollection<Order>? orders { get; set; }
        public virtual Cart? cart { get; set; }
        public virtual Address? address { get; set; }
        public virtual ICollection<Review>? userReviews { get; set; }

    }

    
}
