using System;
namespace VocalSchool.Models
{
    public class DaySubject
    {
        public int DayId { get; set; }
        public virtual Day Day { get; set; }
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        public DaySubject()
        {
        }

        public DaySubject(int dayId, int subjectId)
        {
            DayId = dayId;
            SubjectId = subjectId;   
        }
    }
}
