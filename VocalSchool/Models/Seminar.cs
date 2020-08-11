using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Seminar
    {
        public int SeminarId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4)]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<SeminarDay> SeminarDays { get; set; }
        public virtual ICollection<CourseDesign> CourseDesigns { get; set; }

    }
}
