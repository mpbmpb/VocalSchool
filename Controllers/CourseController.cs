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

            var course = await _db.GetCourseIncludingSubjectsAsync((int)id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            var course = model.Course;
            if (ModelState.IsValid)
            {
                await _db.AddCourseAsync(model);
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

            var course = await _db.GetCourseIncludingSubjectsAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }
            var designs = await _db.GetOnlyCourseDesignsAsync();
            return View(new CourseViewModel(course, designs));
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Name,Description")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            return View(course);
        }

        //GET: Course/AddCourseDates/5
        public async Task<IActionResult> AddCourseDate(int? id)
        {
            var course = await _db.GetCourseIncludingSubjectsAsync((int)id);
            return View(new CourseViewModel(course));
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
    }
}
