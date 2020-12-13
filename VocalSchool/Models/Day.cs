using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace VocalSchool.Models
{
    public class Day : ICourseElement
    {
        public int DayId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        [RegularExpression(@"[^[\]_]*", ErrorMessage = "Characters [ ] and _ are not allowed.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DaySubject> DaySubjects { get; set; }
        public virtual ICollection<SeminarDay> SeminarDays { get; set; }
    }
}
