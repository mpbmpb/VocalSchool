using System;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class CourseDate
    {
        public int CourseDateId { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public string EndTime { get; set; }
        public int VenueId { get; set; }
        public virtual Venue Venue { get; set; }
        public string ReservationInfo { get; set; }
        public string Rider { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
