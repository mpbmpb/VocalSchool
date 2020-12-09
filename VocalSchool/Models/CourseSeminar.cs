namespace VocalSchool.Models
{
    public class CourseSeminar : IMany2Many
    {
        public int CourseDesignId { get; set; }
        public virtual CourseDesign CourseDesign { get; set; }
        public int SeminarId { get; set; }
        public virtual Seminar Seminar { get; set; }

        public int this [int index]
        {
            get
            {
                if (index == 0) return CourseDesignId;
                if (index == 1) return SeminarId;
                return -1;
            }

            set
            {
                if (index == 0) CourseDesignId = value;
                if (index == 1) SeminarId = value;
            }
        }
    }
}
