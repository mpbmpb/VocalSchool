using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VocalSchool.Models
{
    public class Seminar
    {
        public int SeminarId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<SeminarDay> SeminarDays { get; set; }
        public ICollection<CourseDesign> CourseDesigns { get; set; }

    }
}
