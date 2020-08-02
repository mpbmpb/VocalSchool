using System;
namespace VocalSchool.Models
{
    public class CourseSeminar
    {
        public int CourseDesignId { get; set; }
        public CourseDesign CourseDesign { get; set; }
        public int SeminarId { get; set; }
        public Seminar Seminar { get; set; }
    }
}
