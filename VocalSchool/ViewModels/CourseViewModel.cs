using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using VocalSchool.Models;

namespace VocalSchool.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public List<CourseDesign> CourseDesigns { get; set; }
        public List<SelectListItem> DesignList { get; set; }
        public List<CourseDate> CourseDates { get; set; }
        public int DayCount { get; private set; }

        public CourseViewModel()
        {
        }

        public CourseViewModel(List<CourseDesign> designs)
        {
            Course = new Course();
            CourseDesigns = designs;
            DesignList = new List<SelectListItem>();
            DesignList.Add(new SelectListItem { Value = "0", Text = "-- select design --" });

            foreach (var item in designs)
            {
                DesignList.Add(new SelectListItem
                {
                    Value = item.CourseDesignId.ToString(),
                    Text = item.Name
                });

            }
        }
        
        public CourseViewModel(Course course, List<CourseDesign> designs)
            :this(designs)
        {
            Course = course;
        }
        
        public CourseViewModel(Course course, List<CourseDate> dates)
        {
            DayCount = 0;
            Id = course.CourseId;
            CourseDates = new List<CourseDate>();
            foreach (var date in dates)
            {
                if (date.CourseId == Id)
                { CourseDates.Add(date); }
            }
            foreach (var seminar in course.CourseDesign.CourseSeminars)
            {
                DayCount += seminar.Seminar.SeminarDays.Count;
            }
        }

        public CourseViewModel(Course course)
        {
            Course = course;
        }
    }
}
