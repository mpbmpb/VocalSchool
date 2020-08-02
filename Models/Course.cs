﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace VocalSchool.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CourseDesign CourseDesign { get; set; }
        public ICollection<CourseDate> CourseDates { get; set; }
    }
}
