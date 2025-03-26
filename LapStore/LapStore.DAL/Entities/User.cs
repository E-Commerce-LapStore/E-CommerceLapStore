using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    internal class User
    {
        public int Id { get; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public char Role { get; set; }
        public char Gender { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }


    }
}
