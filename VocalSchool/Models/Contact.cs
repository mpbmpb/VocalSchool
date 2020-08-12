using System;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
    }
}
