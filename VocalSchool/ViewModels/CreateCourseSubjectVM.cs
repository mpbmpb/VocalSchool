using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class CreateCourseSubjectVM
    {
        public Subject Subject { get; set; }
        public int CourseDesignId { get; set; }
        
        public string Uid { get; set; }

        public CreateCourseSubjectVM(int courseDesignId, string uid)
        {
            CourseDesignId = courseDesignId;
            Uid = uid;
            Subject = new Subject();
        }

        public CreateCourseSubjectVM()
        {
        }
    }
}