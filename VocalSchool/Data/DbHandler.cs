using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VocalSchool.Models;
using VocalSchool.ViewModels;

namespace VocalSchool.Data
{
    public class DbHandler
    {

        //TODO figure out if this class could cause memory leak without dispose method
        private readonly SchoolContext _context;
        
        public DbHandler(SchoolContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Object entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public T Get<T>(int? id) where T : class
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetAsync<T>(int? id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task RemoveAsync(Object entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Object entity)
        {
                _context.Update(entity);
                await _context.SaveChangesAsync();
        }

        public async Task AddCourseDesignAsync(CourseDesignViewModel model)
        {
            await AddAsync(model.CourseDesign);

            foreach (var item in model.CheckList)
            {
                if (item.IsSelected)
                {
                    CourseSeminar cs = new CourseSeminar()
                    {
                        CourseDesignId = model.CourseDesign.CourseDesignId,
                        SeminarId = item.Id
                    };
                    _context.Add(cs);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddDayAsync(DayViewModel model)
        {
            var day = new Day();
            day.Name = model.Day.Name;
            day.Description = model.Day.Description;
            await AddAsync(day);

            foreach (var item in model.CheckList)
            {
                if (item.IsSelected)
                {
                    DaySubject ds = new DaySubject()
                    {
                        DayId = day.DayId,
                        SubjectId = item.Id
                    };
                    _context.Add(ds);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddCourseAsync(CourseViewModel model)
        {
            var design = _context.CourseDesigns.FirstOrDefault(x => x.CourseDesignId == model.Course.CourseDesign.CourseDesignId);
            model.Course.CourseDesign = design;
            await AddAsync(model.Course);
        }

        public async Task AddSeminarAsync(SeminarViewModel model)
        {
            await AddAsync(model.Seminar);

            var seminarDays = await _context.SeminarDays.ToListAsync();
            foreach (var item in model.CheckList)
            {
                if (item.IsSelected)
                {
                    SeminarDay sd = new SeminarDay()
                    {
                        SeminarId = model.Seminar.SeminarId,
                        DayId = item.Id
                    };
                    _context.Add(sd);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddVenueAsync(VenueViewModel model)
        {
            var contacts = await _context.Contacts.ToListAsync();

            if (model.Venue.Contact1 != null)
            {
                var con1 = contacts.FirstOrDefault(x => x.ContactId == model.Venue.Contact1.ContactId);
                model.Venue.Contact1 = con1;
            }
            if (model.Venue.Contact2 != null)
            {
                var con2 = contacts.FirstOrDefault(x => x.ContactId == model.Venue.Contact2.ContactId);
                model.Venue.Contact2 = con2;
            }
            _context.Venues.Add(model.Venue);
            await _context.SaveChangesAsync();
        }

        public bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ContactId == id);
        }

        public async Task<Course> GetCourseIncludingSubjectsAsync(int id)
        {
            return await _context.Courses
                .Where(c => c.CourseId == id)
                .Include(c => c.CourseDates)
                .Include(c => c.CourseDesign)
                .ThenInclude(cd => cd.CourseSeminars)
                .ThenInclude(cs => cs.Seminar)
                .ThenInclude(s => s.SeminarDays)
                .ThenInclude(sd => sd.Day)
                .ThenInclude(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.CourseDates)
                .Include(c => c.CourseDesign)
                .ThenInclude(cd => cd.CourseSeminars)
                .ThenInclude(cs => cs.Seminar)
                .ThenInclude(s => s.SeminarDays)
                .ThenInclude(sd => sd.Day)
                .ThenInclude(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .ToListAsync();
        }

        public async Task<CourseDesign> GetCourseDesignFullAsync(int? id)
        {
            return await _context.CourseDesigns
                .Where(cd => cd.CourseDesignId == id)
                .Include(c => c.CourseSeminars)
                .ThenInclude(cs => cs.Seminar)
                .ThenInclude(s => s.SeminarDays)
                .ThenInclude(sd => sd.Day)
                .ThenInclude(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CourseDesign>> GetAllCourseDesignsAsync()
        {
            return await _context.CourseDesigns
                .Include(cd => cd.CourseSeminars)
                .ThenInclude(cs => cs.Seminar)
                .ThenInclude(s => s.SeminarDays)
                .ThenInclude(sd => sd.Day)
                .ThenInclude(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .ToListAsync();
        }

        public async Task<List<CourseDesign>> GetOnlyCourseDesignsAsync()
        {
            return await _context.CourseDesigns.ToListAsync();
        }

        public async Task<Day> GetDayFullAsync(int? id)
        {
            return await _context.Days
                .Include(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .FirstOrDefaultAsync(d => d.DayId == id);
        }

        public async Task<List<Day>> GetAllDaysAsync()
        {
            return await _context.Days
                //.Include(d => d.DaySubjects)
                //.ThenInclude(ds => ds.Subject)
                .OrderBy(d => d.Name.ToLower())
                .ToListAsync();
        }

        public async Task<List<DaySubject>> GetAllDaySubjectsAsync()
        {
            return await _context.DaySubjects.ToListAsync();
        }

        public async Task<Seminar> GetSeminarAsync(int id)
        {
            return await _context.Seminars
                .Include(s => s.SeminarDays)
                .FirstOrDefaultAsync(x => x.SeminarId == id);
        }

        public async Task<Seminar> GetSeminarIncludingSubjectsAsync(int id)
        {
            return await _context.Seminars
               .Where(s => s.SeminarId == id)
               .Include(d => d.SeminarDays)
               .ThenInclude(sd => sd.Day)
               .ThenInclude(x => x.DaySubjects)
               .ThenInclude(y => y.Subject)
               .FirstOrDefaultAsync();
        }

        public async Task<List<Seminar>> GetAllSeminarsFullAsync()
        {
            return await _context.Seminars
               .Include(s => s.SeminarDays)
               .ThenInclude(sd => sd.Day)
               .ThenInclude(d => d.DaySubjects)
               .ThenInclude(ds => ds.Subject)
               .OrderBy(s => s.Name.ToLower())
               .ToListAsync();
        }

        public async Task<Subject> GetSubjectAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsIncludeDaysAsync()
        {
            return await _context.Subjects
                .Include(s => s.DaySubjects)
                .ThenInclude(ds => ds.Day)
                .ToListAsync();
        }

        public async Task<Venue> GetVenueAsync(int id)
        {
            return await _context.Venues
                .Where(v => v.VenueId == id)
                .Include(v => v.Contact1)
                .Include(v => v.Contact2)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Venue>> GetAllVenuesAsync()
        {
            return await _context.Venues
                .Include(v => v.Contact1)
                .Include(v => v.Contact2)
                .ToListAsync();
        }

        public async Task UpdateVenueAsync(VenueViewModel model)
        {
            var con1 = _context.Contacts.FirstOrDefault(x => x.ContactId == model.Venue.Contact1.ContactId);
            var con2 = _context.Contacts.FirstOrDefault(x => x.ContactId == model.Venue.Contact2.ContactId);
            model.Venue.Contact1 = con1;
            model.Venue.Contact2 = con2;

            await UpdateAsync(model.Venue);
        }

        public async Task UpdateCourseDesignAsync(CourseDesignViewModel model)
        {
            await UpdateAsync(model.CourseDesign);
            var courseSeminars = await _context.CourseSeminars.ToListAsync();

            model.CheckList.ForEach(async item =>
            {
                var existingEntry = courseSeminars
                    .FirstOrDefault(x => x.CourseDesignId == model.CourseDesign.CourseDesignId
                    && x.SeminarId == item.Id);
                if (!item.IsSelected && existingEntry != null)
                {
                    await RemoveAsync(existingEntry);
                }

                if (item.IsSelected && existingEntry == null)
                {
                    CourseSeminar cs = new CourseSeminar()
                    {
                        CourseDesignId = model.CourseDesign.CourseDesignId,
                        SeminarId = item.Id
                    };
                    _context.Add(cs);
                }
            });
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDayAsync(DayViewModel model)
        {
            await UpdateAsync(model.Day);
            var daySubjects = await GetAllDaySubjectsAsync();

            model.CheckList.ForEach(async item =>
            {
                var existingEntry = daySubjects
                    .FirstOrDefault(x => x.DayId == model.Day.DayId && x.SubjectId == item.Id);
                if (!item.IsSelected && existingEntry != null)
                {
                    await RemoveAsync(existingEntry);
                }

                if (item.IsSelected && existingEntry == null)
                {
                    DaySubject ds = new DaySubject()
                    {
                        DayId = model.Day.DayId,
                        SubjectId = item.Id
                    };
                    _context.Add(ds);
                }
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateSeminarAsync(SeminarViewModel model)
        {
            await UpdateAsync(model.Seminar);
            var seminarDays = await _context.SeminarDays.ToListAsync();

            //TODO: this fixes null exception being triggered at start of function
            // find the others
            model.CheckList.ForEach(async item => 
           {
               var existingEntry = seminarDays
                   .FirstOrDefault(x => x.SeminarId == model.Seminar.SeminarId && x.DayId == item.Id);
               if (!item.IsSelected && existingEntry != null)
               {
                   await RemoveAsync(existingEntry);
               }

               if (item.IsSelected && existingEntry == null)
               {
                   SeminarDay ds = new SeminarDay()
                   {
                       SeminarId = model.Seminar.SeminarId,
                       DayId = item.Id
                   };
                   _context.Add(ds);
               }
           });

            await _context.SaveChangesAsync();
        }


    }
}
