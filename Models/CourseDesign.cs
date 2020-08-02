using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class CourseDesign
    {
        public int CourseDesignId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<CourseSeminar> CourseSeminars { get; set; }
    }
}
