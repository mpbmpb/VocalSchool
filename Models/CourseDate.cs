using System;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class CourseDate
    {
        public int CourseDateId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int VenueId { get; set; }
        public Venue Venue { get; set; }
        public string ReservationInfo { get; set; }
        public string Rider { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
