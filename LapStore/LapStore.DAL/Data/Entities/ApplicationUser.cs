using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LapStore.DAL.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("address")]
        public int AddressId { get; set; }
        public virtual Address? Address { get; set; }

    }
}