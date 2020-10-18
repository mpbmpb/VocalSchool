using System.Collections.Generic;

namespace VocalSchool.Models
{ 
    public class CourseProxy
    {
        private Course _course;

        public CourseProxy(Course course)
        {
            _course = course;
        }
        public ICollection<CourseSeminar> Seminars => _course.CourseDesign.CourseSeminars;

    }
    
}