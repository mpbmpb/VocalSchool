using System.Collections.Generic;
using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class DayViewModel
    {
        public Day Day { get; set; }
        public List<CheckedId> CheckList { get; set; }
        public string Uid { get; private set; }
        public string LastPage { get; set; }

        public DayViewModel()
        {
        }

        public DayViewModel(Day day, string lastPage)
        {
            Day = day;
            LastPage = lastPage;
        }

        public DayViewModel(List<Subject> subjects, string lastPage)
        {
            Day = new Day();
            CheckList = new List<CheckedId>();
            LastPage = lastPage;

            foreach (var subject in subjects)
            {
                var check = new CheckedId()
                {
                    Id = subject.SubjectId,
                    Name = subject.Name,
                    Description = subject.Description,
                    IsSelected = false
                };
                CheckList.Add(check);
            }
        }

        public DayViewModel(Day day, IEnumerable<Subject> subjects)
        {
            Day = day;
            CheckList = new List<CheckedId>();

            foreach (var subject in subjects)
            {
                bool isInDaySubjects = (Day.DaySubjects.Any(x => x.SubjectId == subject.SubjectId));

                var check = new CheckedId()
                {
                    Id = subject.SubjectId,
                    Name = subject.Name,
                    Description = subject.Description,
                    IsSelected = isInDaySubjects
                };

                CheckList.Add(check);
            }
        }
        public DayViewModel(Day day, IEnumerable<Subject> subjects, string uid, string lastPage)
        {
            Day = day;
            CheckList = new List<CheckedId>();
            Uid = uid;
            LastPage = lastPage;

            foreach (var subject in subjects)
            {
                bool isInDaySubjects = (Day.DaySubjects.Any(x => x.SubjectId == subject.SubjectId));

                var check = new CheckedId()
                {
                    Id = subject.SubjectId,
                    Name = subject.Name,
                    Description = subject.Description,
                    IsSelected = isInDaySubjects
                };

                CheckList.Add(check);
            }
        }
    }
}
