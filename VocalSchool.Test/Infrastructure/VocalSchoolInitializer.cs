using System.Linq;
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
                new Seminar { SeminarId = 2, Name = "Seminar1",
                    Description = "Diving deeper in the modes."},
                new Seminar { SeminarId = 3, Name = "Seminar1",
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
        }
    }
}
