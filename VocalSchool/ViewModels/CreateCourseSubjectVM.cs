using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class CreateCourseSubjectVM
    {
        public Subject Subject { get; set; }
        public int CourseId { get; set; }
        
        public string Uid { get; set; }

        public CreateCourseSubjectVM(int courseId, string uid)
        {
            CourseId = courseId;
            Uid = uid;
            Subject = new Subject();
        }

        public CreateCourseSubjectVM()
        {
            Subject = new Subject();
        }
    }
}