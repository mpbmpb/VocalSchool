﻿using System;
namespace VocalSchool.Models
{
    public class DaySubject
    {
        public int DayId { get; set; }
        public Day Day { get; set; }
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
