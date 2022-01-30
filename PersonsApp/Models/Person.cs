using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonsApp.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
