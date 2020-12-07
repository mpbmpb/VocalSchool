using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;

namespace VocalSchool.Controllers
{
    public class CourseController : Controller
    {
        private readonly SchoolContext _context;
        private readonly DbHandler _db;

        public CourseController(SchoolContext context)
        {
            _context = context;
            _db = new DbHandler(context);
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetAllCoursesAsync());
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _db.GetCourseFullAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Create
        public async Task<IActionResult> Create()
        {
            return View(new CourseViewModel(await _db.GetOnlyCourseDesignsAsync()));
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            var course = model.Course;
            if (ModelState.IsValid)
            {
                await _db.AddCourseAsync(model);
                model = await CopyCourseDesignAsync(model);
                await _db.UpdateCourseAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _db.GetCourseFullAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }
            var designs = await _db.GetOnlyCourseDesignsAsync();
            return View(new CourseViewModel(course, designs));
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (id != model.Course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _db.UpdateCourseAsync(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(model.Course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        //GET: Course/AddCourseDates/5
        public async Task<IActionResult> AddCourseDates(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _db.GetCourseFullAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }
            var courseDates = await _db.GetAllAsync<CourseDate>();
            return View(new CourseViewModel(course, courseDates));
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> AddCourseDates(int? id, CourseViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _db.GetCourseFullAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _db.AddCourseDatesAsync(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(model.Course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        public async Task<CourseViewModel> CopyCourseDesignAsync(CourseViewModel model)
        {
            var cd = await _db.GetCourseDesignFullAsync(model.Course.CourseDesign.CourseDesignId);
            var uid = $"[{model.Course.Name}-{model.Course.CourseId}]";
            var courseDesign = new CourseDesign();
            courseDesign.Name = cd.Name.Prepend(uid);
            courseDesign.Description = cd.Description;
            await _db.AddAsync(courseDesign);
           
            foreach (var courseSeminar in cd.CourseSeminars)
            {
                var seminar = new Seminar();
                seminar.Name = courseSeminar.Seminar.Name.Prepend(uid);
                seminar.Description = courseSeminar.Seminar.Description;
                await _db.AddAsync(seminar);

                var cs = new CourseSeminar();
                cs.CourseDesignId = courseDesign.CourseDesignId;
                cs.SeminarId = seminar.SeminarId;
                await _db.AddAsync(cs);
                
                foreach (var seminarDay in courseSeminar.Seminar.SeminarDays)
                {
                    var day = new Day();
                    day.Name = seminarDay.Day.Name.Prepend(uid);
                    day.Description = seminarDay.Day.Description;
                    await _db.AddAsync(day);

                    var sd = new SeminarDay();
                    sd.SeminarId = seminar.SeminarId;
                    sd.DayId = day.DayId;
                    await _db.AddAsync(sd);
                    
                    foreach (var daySubject in seminarDay.Day.DaySubjects)
                    {
                        var subject = new Subject();
                        subject.Name = daySubject.Subject.Name.Prepend(uid);
                        subject.Description = daySubject.Subject.Description;
                        subject.RequiredReading = daySubject.Subject.RequiredReading;
                        await _db.AddAsync(subject);
                        
                        var ds = new DaySubject();
                        ds.DayId = day.DayId;
                        ds.SubjectId = subject.SubjectId;
                        await _db.AddAsync(ds);
                    }
                }
            }
            model.Course.CourseDesign = courseDesign;
            return model;
        }
    }

    public static class Extensions
    {
        public static string Prepend(this string s, string pre) => $"{pre} {s}";
    }
}
