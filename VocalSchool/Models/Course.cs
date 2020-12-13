using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        [RegularExpression(@"[^[\]_]", ErrorMessage = "Characters [ ] and _ are not allowed.")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "You have to pick a Course Design")]
        public virtual CourseDesign CourseDesign { get; set; }
        public virtual ICollection<CourseDate> CourseDates { get; set; }
    }
}
