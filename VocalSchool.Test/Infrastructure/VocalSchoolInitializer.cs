using System.Linq;
using VocalSchool.Models;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            if (!context.Subjects.Any()) { return; }


            Seed(context);

        }

        private static void Seed(SchoolContext context)
        {
            var customers = new[]
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

            context.Subjects.AddRange(customers);
            context.SaveChanges();
        }
    }
}
