using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        public string Name { get; set; }
        public string Description { get; set; }

        public int MaxStudents { get; set; } = 12;
        
        [Required(ErrorMessage = "You have to pick a Course Design")]
        public virtual CourseDesign CourseDesign { get; set; }
        public virtual ICollection<CourseDate> CourseDates { get; set; }
        
        public virtual ICollection<Change> Changes { get; set; }

        public CourseProxy Details;

        public Course()
        {
            Details = new CourseProxy(this);
        }
    }
}
