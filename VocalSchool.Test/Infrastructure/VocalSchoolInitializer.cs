﻿using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            if (context.Subjects.Any()) { return; }


            Seed(context);

        }

        private static void Seed(SchoolContext context)
        {
            var subjects = new[]
            {
                new Subject { SubjectId = 1, Name = "Introduction",
                    Description = "What is cvt and how is it structured.", RequiredReading = "CVT App pages 3-8"},
                new Subject { SubjectId = 2, Name = "Overview",
                    Description = "Overview of cvt, it's terms and structure.", RequiredReading = "CVT App pages 9-14"},
                new Subject { SubjectId = 3, Name = "Support",
                    Description = "Introduction into support, and how to use it.", RequiredReading = "CVT App pages 15-24"},
                new Subject { SubjectId = 4, Name = "Neutral",
                    Description = "Introduction into neutral, and how to use it.", RequiredReading = "CVT App pages 52-61"},
                new Subject { SubjectId = 5, Name = "Overdrive",
                    Description = "Introduction into overdrive, and how to use it.", RequiredReading = "CVT App pages 85-89"},
                new Subject { SubjectId = 6, Name = "Edge",
                    Description = "Introduction into edge, and how to use it.", RequiredReading = "CVT App pages 76-80"},           
            };

            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            var days = new[]
            {
                new Day { DayId = 1, Name = "Day1",
                    Description = "Introduction day. Overview of the system, overall principles."},
                new Day { DayId = 2, Name = "Day2",
                    Description = "The modes Neutral and Overdrive."},
                new Day { DayId = 3, Name = "Day3",
                    Description = "Deeper in the modes."},
                new Day { DayId = 4, Name = "Day4",
                    Description = "Edge"},
                new Day { DayId = 5, Name = "Day5",
                    Description = "All the modes."},
                new Day { DayId = 6, Name = "Day6",
                    Description = "Recap, practise & questions."},           
            };

            context.Days.AddRange(days);
            context.SaveChanges();

            var daySubjects = new[]
            {
                new DaySubject() { DayId = 1, SubjectId = 1 },
                new DaySubject() { DayId = 1, SubjectId = 2 },
                new DaySubject() { DayId = 1, SubjectId = 3 },
                new DaySubject() { DayId = 2, SubjectId = 4 },
                new DaySubject() { DayId = 2, SubjectId = 5 },
                new DaySubject() { DayId = 3, SubjectId = 3 },
                new DaySubject() { DayId = 3, SubjectId = 4 },
                new DaySubject() { DayId = 3, SubjectId = 5 },
                new DaySubject() { DayId = 4, SubjectId = 6 },
                new DaySubject() { DayId = 5, SubjectId = 4 },
                new DaySubject() { DayId = 5, SubjectId = 5 },
                new DaySubject() { DayId = 5, SubjectId = 6 },
                new DaySubject() { DayId = 6, SubjectId = 3 },
                new DaySubject() { DayId = 6, SubjectId = 4 },
                new DaySubject() { DayId = 6, SubjectId = 5 },
                new DaySubject() { DayId = 6, SubjectId = 6 },
            };

            context.DaySubjects.AddRange(daySubjects);
            context.SaveChanges();

            var seminars = new[]
            {
                new Seminar { SeminarId = 1, Name = "Seminar1",
                    Description = "Introduction seminar."},
                new Seminar { SeminarId = 2, Name = "Seminar2",
                    Description = "Diving deeper in the modes."},
                new Seminar { SeminarId = 3, Name = "Seminar3",
                    Description = "Recap, questions and eval."},
                
            };

            context.Seminars.AddRange(seminars);
            context.SaveChanges();

            var seminarDays = new[]
            {
                new SeminarDay() {SeminarId = 1, DayId = 1},
                new SeminarDay() {SeminarId = 1, DayId = 2},
                new SeminarDay() {SeminarId = 2, DayId = 3},
                new SeminarDay() {SeminarId = 2, DayId = 4},
                new SeminarDay() {SeminarId = 3, DayId = 5},
                new SeminarDay() {SeminarId = 3, DayId = 6},
            };

            context.SeminarDays.AddRange(seminarDays);
            context.SaveChanges();

            var courseDesigns = new[]
            {
                new CourseDesign() {CourseDesignId = 1, Name = "CourseDesign1",
                    Description = "One weekend workshop"},
                new CourseDesign() {CourseDesignId = 2, Name = "CourseDesign2",
                    Description = "Short 4 day introduction course."},
                new CourseDesign() {CourseDesignId = 3, Name = "CourseDesign3",
                    Description = "Complete 3 seminars introduction course."},
            };

            context.CourseDesigns.AddRange(courseDesigns);
            context.SaveChanges();

            var courseSeminars = new[]
            {
                new CourseSeminar() {CourseDesignId = 1, SeminarId = 1},
                new CourseSeminar() {CourseDesignId = 2, SeminarId = 1},
                new CourseSeminar() {CourseDesignId = 2, SeminarId = 2},
                new CourseSeminar() {CourseDesignId = 3, SeminarId = 1},
                new CourseSeminar() {CourseDesignId = 3, SeminarId = 2},
                new CourseSeminar() {CourseDesignId = 3, SeminarId = 3},
            };

            context.CourseSeminars.AddRange(courseSeminars);
            context.SaveChanges();

            var contacts = new[]
            {
                new Contact() {ContactId = 1, Name = "Contact1", Email = "mitch@acme.com",
                    Phone = "06-0001", Adress = "polanenbuurt 1 1000BB Amsterdam"},
                new Contact() {ContactId = 2, Name = "Contact2", Email = "peter@acme.com",
                    Phone = "06-0002", Adress = "polanenbuurt 2 1000BB Amsterdam"},
                new Contact() {ContactId = 3, Name = "Contact3", Email = "jenny@acme.com",
                    Phone = "06-0003", Adress = "Atlantisplein 2 1000XX Amsterdam"}
            };

            context.Contacts.AddRange(contacts);
            context.SaveChanges();

            var venues = new[]
            {
                new Venue() {VenueId = 1, Name = "Venue1",
                    Info = "Van alles over de bel en de poort etc.", Email1 = "info@polanen.acme",
                    Phone = "020-1205", Adress = "polanenstraat 1 1000AA Amsterdam",
                    MapsUrl = "https://goo.gl/maps/qMjMmcgD43nf9Fqy9"},
                new Venue() {VenueId = 2, Name = "Venue2", Info = "Alles checken, klopt altijd wel iets niet!",
                    Email1 = "info@qfactory.acme", Email2 = "sorry@qfactory.acme",
                    Phone = "020-1206", Adress = "Atlantisplein 1 1000XX Amsterdam",
                    MapsUrl = "https://goo.gl/maps/u3isDnp99GXWZ23U7"}
            };

            context.Venues.AddRange(venues);
            context.SaveChanges();

            Venue venue1 = context.Venues.FirstOrDefault(x => x.VenueId == 1);
            venue1.Contact1 = contacts[0];
            venue1.Contact2 = contacts[1];
            Venue venue2 = context.Venues.FirstOrDefault(x => x.VenueId == 2);
            venue2.Contact1 = contacts[2];
            context.Update(venue1);
            context.Update(venue2);
            context.SaveChanges();
        }
    }
}
