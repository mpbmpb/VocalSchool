using System;
namespace VocalSchool.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string MapsUrl { get; set; }
        public Contact Contact1 { get; set; }
        public Contact Contact2 { get; set; }

    }
}
