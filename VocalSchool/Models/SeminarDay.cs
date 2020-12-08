using System;
namespace VocalSchool.Models
{
    public class SeminarDay : IMany2Many
    {
        public int SeminarId { get; set; }
        public virtual Seminar Seminar { get; set; }
        public int DayId { get; set; }
        public virtual Day Day { get; set; }

        public int this [int index]
        {
            get
            {
                if (index == 0) return SeminarId;
                if (index == 1) return DayId;
                return -1;
            }

            set
            {
                if (index == 0) SeminarId = value;
                if (index == 1) DayId = value;
            }
        }
    }
}
