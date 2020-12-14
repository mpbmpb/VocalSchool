using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class SeminarViewModel
    {
        [Required]
        public Seminar Seminar { get; set; }
        public List<CheckedId> CheckList { get; set; }
        public List<Day> Days { get; set; }
        public string Uid { get; private set; }
        public string LastPage { get; set; }


        public SeminarViewModel()
        {
        }

        public SeminarViewModel(Seminar seminar, string lastPage)
        {
            Seminar = seminar;
            LastPage = lastPage;
        }

        public SeminarViewModel(List<Day> days)
        {
            Seminar = new Seminar();
            CheckList = new List<CheckedId>();
            Days = days;

            foreach (var day in days)
            {
                var check = new CheckedId()
                {
                    Id = day.DayId,
                    Name = day.Name,
                    Description = day.Description,
                    IsSelected = false
                };
                CheckList.Add(check);
            }
        }

        public SeminarViewModel(List<Day> days, string lastPage)
        {
            Seminar = new Seminar();
            CheckList = new List<CheckedId>();
            Days = days;
            LastPage = lastPage;

            foreach (var day in days)
            {
                var check = new CheckedId()
                {
                    Id = day.DayId,
                    Name = day.Name,
                    Description = day.Description,
                    IsSelected = false
                };
                CheckList.Add(check);
            }
        }

        public SeminarViewModel(Seminar seminar, List<Day> days)
        {
            Seminar = seminar;
            CheckList = new List<CheckedId>();
            Days = days;

            foreach (var day in days)
            {
                bool isInSeminarDays = (Seminar.SeminarDays.Any(x => x.DayId == day.DayId));

                var check = new CheckedId()
                {
                    Id = day.DayId,
                    Name = day.Name,
                    Description = day.Description,
                    IsSelected = isInSeminarDays
                };
                CheckList.Add(check);
            }
        }
        
        public SeminarViewModel(Seminar seminar, List<Day> days, string uid)
        {
            Seminar = seminar;
            CheckList = new List<CheckedId>();
            Days = days;
            Uid = uid;

            foreach (var day in days)
            {
                bool isInSeminarDays = (Seminar.SeminarDays.Any(x => x.DayId == day.DayId));

                var check = new CheckedId()
                {
                    Id = day.DayId,
                    Name = day.Name,
                    Description = day.Description,
                    IsSelected = isInSeminarDays
                };
                CheckList.Add(check);
            }
        }
        public SeminarViewModel(Seminar seminar, List<Day> days, string uid, string lastPage)
        {
            Seminar = seminar;
            CheckList = new List<CheckedId>();
            Days = days;
            Uid = uid;
            LastPage = lastPage;

            foreach (var day in days)
            {
                bool isInSeminarDays = (Seminar.SeminarDays.Any(x => x.DayId == day.DayId));

                var check = new CheckedId()
                {
                    Id = day.DayId,
                    Name = day.Name,
                    Description = day.Description,
                    IsSelected = isInSeminarDays
                };
                CheckList.Add(check);
            }
        }
    }
}
