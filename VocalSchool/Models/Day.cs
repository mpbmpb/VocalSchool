using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace VocalSchool.Models
{
    public class Day
    {
        public int DayId { get; set; }
        [Required(ErrorMessage = "Name of at least 4 characters is required.")]
        [MinLength(4, ErrorMessage = "Name should be at least 4 characters long.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DaySubject> DaySubjects { get; set; }
        public virtual ICollection<SeminarDay> SeminarDays { get; set; }
    }
}
