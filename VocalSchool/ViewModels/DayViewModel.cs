using System.Collections.Generic;
using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class DayViewModel
    {
        public Day Day { get; set; }
        public List<CheckedId> CheckList { get; set; }

        public DayViewModel()
        {
        }

        public DayViewModel(List<Subject> subjects)
        {
            Day = new Day();
            CheckList = new List<CheckedId>();

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
    }
}
