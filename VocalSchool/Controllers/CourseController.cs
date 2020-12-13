using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var courseDesigns = await _db.GetOnlyCourseDesignsAsync(x => x.Name.Substring(0, 1) != "[");
            return View(new CourseViewModel(courseDesigns));
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
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
            var designs = new List<CourseDesign>() { course.CourseDesign };

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
                    var courseDesign = await _db.GetCourseDesignFullAsync(model.Course.CourseDesign.CourseDesignId);
                    var uid = courseDesign.GetUid();
                    var end = uid.IndexOf('_') - 1;
                    if (uid != "" && model.Course.Name != uid.Substring(1, end) )
                        model = await CopyCourseDesignAsync(model);

                    await _db.UpdateCourseAsync(model);
                    
                    if (model.Course.CourseDesign.GetUid() != uid)
                        await _db.RemoveAllCourseDesignElementsAsync(uid);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(model.Course.CourseId))
                    {
                        return NotFound();
                    }
                    throw;
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

            var course = await _db.GetCourseFullAsync((int)id);
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
            var course = await _db.GetCourseFullAsync(id);
            await _db.RemoveCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        private async Task<CourseViewModel> CopyCourseDesignAsync(CourseViewModel model)
        {
            var cd = await _db.GetCourseDesignFullAsync(model.Course.CourseDesign.CourseDesignId);
            var uid = $"[{model.Course.Name}_{model.Course.CourseId}]";
            var courseDesign = await cd.CopyAndPrependNameWithAsync(uid, _db);
           
            foreach (var courseSeminar in cd.CourseSeminars)
            {
                var seminar = await courseSeminar.Seminar.CopyAndPrependNameWithAsync(uid, _db);
                await courseSeminar.CreateMany2ManyAsync(courseDesign.CourseDesignId, seminar.SeminarId, _db);
                
                foreach (var seminarDay in courseSeminar.Seminar.SeminarDays)
                {
                    var day = await seminarDay.Day.CopyAndPrependNameWithAsync(uid, _db);
                    await seminarDay.CreateMany2ManyAsync(seminar.SeminarId, day.DayId, _db);
                    
                    foreach (var daySubject in seminarDay.Day.DaySubjects)
                    {
                        var subject = await daySubject.Subject.CopyAndPrependNameWithAsync(uid, _db);
                        await daySubject.CreateMany2ManyAsync(day.DayId, subject.SubjectId, _db);
                    }
                }
            }
            model.Course.CourseDesign = courseDesign;
            return model;
        }
    }
}
