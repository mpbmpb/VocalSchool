using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        public string Name { get; set; }
        public string Info { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string MapsUrl { get; set; }
        public virtual Contact Contact1 { get; set; }
        public virtual Contact Contact2 { get; set; }

    }
}
