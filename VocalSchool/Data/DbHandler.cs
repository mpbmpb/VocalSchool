using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VocalSchool.Controllers;
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

        public async Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task RemoveAsync(Object entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        
        public async Task RemoveCourseAsync(Course course)
        {
            var uid = course.CourseDesign.Name.GetUid();
            if (uid != "")
            {
                foreach (var courseSeminar in course.CourseDesign.CourseSeminars)
                {
                    foreach (var seminarDay in courseSeminar.Seminar.SeminarDays)
                    {
                        foreach (var daySubject in seminarDay.Day.DaySubjects)
                        {
                            if (daySubject.Subject.Name.Length >= uid.Length
                                && daySubject.Subject.Name.Substring(0, uid.Length) == uid)
                                _context.Remove(daySubject.Subject);
                        }

                        if (seminarDay.Day.Name.Length >= uid.Length
                            && seminarDay.Day.Name.Substring(0, uid.Length) == uid)
                            _context.Remove(seminarDay.Day);
                    }

                    if (courseSeminar.Seminar.Name.Length >= uid.Length
                        && courseSeminar.Seminar.Name.Substring(0, uid.Length) == uid)
                        _context.Remove(courseSeminar.Seminar);
                }

                if (course.CourseDesign.Name.Length >= uid.Length
                    && course.CourseDesign.Name.Substring(0, uid.Length) == uid)
                    _context.Remove(course.CourseDesign);
            }

            _context.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCourseDesignElementsAsync(CourseDesign courseDesign)
        {
            var uid = courseDesign.Name.GetUid();
            if (uid != "")
            {
                foreach (var courseSeminar in courseDesign.CourseSeminars)
                {
                    foreach (var seminarDay in courseSeminar.Seminar.SeminarDays)
                    {
                        foreach (var daySubject in seminarDay.Day.DaySubjects)
                        {
                            if (daySubject.Subject.Name.Length >= uid.Length
                                && daySubject.Subject.Name.Substring(0, uid.Length) == uid)
                                _context.Remove(daySubject.Subject);
                        }

                        if (seminarDay.Day.Name.Length >= uid.Length
                            && seminarDay.Day.Name.Substring(0, uid.Length) == uid)
                            _context.Remove(seminarDay.Day);
                    }

                    if (courseSeminar.Seminar.Name.Length >= uid.Length
                        && courseSeminar.Seminar.Name.Substring(0, uid.Length) == uid)
                        _context.Remove(courseSeminar.Seminar);
                }
            }

            _context.Remove(courseDesign);
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

        public async Task AddCourseDatesAsync(CourseViewModel model)
        {
            foreach (var date in model.CourseDates)
            {
                if (CourseDateExists(date.CourseDateId))
                {
                    var existing = _context.CourseDates.Find(date.CourseDateId);
                    _context.Remove(existing);
                    await _context.SaveChangesAsync();
                    _context.Add(date);
                }
                else
                {
                    _context.Add(date);
                }
            }
            await _context.SaveChangesAsync();
        }

        public bool CourseDateExists(int id)
        {
            return _context.CourseDates.Any(e => e.CourseDateId == id);
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
            if (model.Course.CourseDesign != null)
            {
                var design = _context.CourseDesigns
                    .FirstOrDefault(x => x.CourseDesignId == model.Course.CourseDesign.CourseDesignId);
                model.Course.CourseDesign = design;
            }
            await AddAsync(model.Course);
        }

        public async Task AddSeminarAsync(SeminarViewModel model)
        {
            await AddAsync(model.Seminar);
            
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

        public async Task<Course> GetCourseFullAsync(int id)
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

        public async Task<CourseDesign> GetCourseDesignFullAsync(int? id, Expression<Func<CourseDesign, bool>> predicate)
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

        public async Task<List<CourseDesign>> GetAllCourseDesignsFullAsync()
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
        
        public async Task<List<CourseDesign>> GetAllCourseDesignsFullAsync(Expression<Func<CourseDesign, bool>> predicate)
        {
            return await _context.CourseDesigns
                .Where(predicate)
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

        public async Task<List<CourseDesign>> GetOnlyCourseDesignsAsync(Expression<Func<CourseDesign, bool>> predicate)
        {
            return await _context.CourseDesigns.Where(predicate).ToListAsync();
        }

        public async Task<Day> GetDayFullAsync(int? id)
        {
            return await _context.Days
                .Include(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .FirstOrDefaultAsync(d => d.DayId == id);
        }

        public async Task<List<Day>> GetAllDaysFullAsync()
        {
            return await _context.Days
                .Include(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .OrderBy(d => d.Name.ToLower())
                .ToListAsync();
        }

        public async Task<List<Day>> GetAllDaysFullAsync(Expression<Func<Day, bool>> predicate)
        {
            return await _context.Days
                .Where(predicate)
                .Include(d => d.DaySubjects)
                .ThenInclude(ds => ds.Subject)
                .OrderBy(d => d.Name.ToLower())
                .ToListAsync();
        }

        public async Task<List<DaySubject>> GetAllDaySubjectsAsync()
        {
            return await _context.DaySubjects.ToListAsync();
        }

        public async Task<Seminar> GetSeminarAndDaysAsync(int? id)
        {
            return await _context.Seminars
                .Include(s => s.SeminarDays)
                .FirstOrDefaultAsync(x => x.SeminarId == id);
        }

        public async Task<Seminar> GetSeminarFullAsync(int? id)
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

        public async Task<List<Seminar>> GetAllSeminarsFullAsync(Expression<Func<Seminar, bool>> predicate)
        {
            return await _context.Seminars
                .Where(predicate)
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
        
        public async Task UpdateCourseAsync(CourseViewModel model)
        {
            var design = _context.CourseDesigns.FirstOrDefault(x =>
                x.CourseDesignId == model.Course.CourseDesign.CourseDesignId);
            model.Course.CourseDesign = design;

            await UpdateAsync(model.Course);
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
