using System;
namespace VocalSchool.Models
{
    public class SeminarDay
    {
        public int SeminarId { get; set; }
        public Seminar Seminar { get; set; }
        public int DayId { get; set; }
        public Day Day { get; set; }
    }
}
