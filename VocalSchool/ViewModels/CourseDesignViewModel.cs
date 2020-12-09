using System.Collections.Generic;
using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class CourseDesignViewModel
    {
        public CourseDesign CourseDesign { get; set; }
        public List<CheckedId> CheckList { get; set; }
        public List<Seminar> Seminars { get; set; }

        public CourseDesignViewModel()
        {
        }

        public CourseDesignViewModel(List<Seminar> seminars)
        {
            CourseDesign = new CourseDesign();
            CheckList = new List<CheckedId>();
            Seminars = seminars;

            foreach (var sem in seminars)
            {
                var check = new CheckedId()
                {
                    Id = sem.SeminarId,
                    Name = sem.Name,
                    Description = sem.Description,
                    IsSelected = false
                };
                CheckList.Add(check);
            }
        }

        public CourseDesignViewModel(CourseDesign courseDesign, List<Seminar> seminars)
        {
            CourseDesign = courseDesign;
            CheckList = new List<CheckedId>();
            Seminars = seminars;

            foreach (var sem in seminars)
            {
                bool isInCourseSeminars = (CourseDesign.CourseSeminars.Any(x => x.SeminarId == sem.SeminarId));

                var check = new CheckedId()
                {
                    Id = sem.SeminarId,
                    Name = sem.Name,
                    Description = sem.Description,
                    IsSelected = isInCourseSeminars
                };
                CheckList.Add(check);
            }
        }
    }
}
