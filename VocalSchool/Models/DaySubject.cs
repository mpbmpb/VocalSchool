namespace VocalSchool.Models
{
    public class DaySubject : IMany2Many
    {
        public int DayId { get; set; }
        public virtual Day Day { get; set; }
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }

        public int this [int index]
        {
            get
            {
                if (index == 0) return DayId;
                if (index == 1) return SubjectId;
                return -1;
            }

            set
            {
                if (index == 0) DayId = value;
                if (index == 1) SubjectId = value;
            }
        }
    }
}
