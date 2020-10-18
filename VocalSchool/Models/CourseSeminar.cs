namespace VocalSchool.Models
{
    public class CourseSeminar
    {
        public int CourseDesignId { get; set; }
        public virtual CourseDesign CourseDesign { get; set; }
        public int SeminarId { get; set; }
        public virtual Seminar Seminar { get; set; }
    }
}
