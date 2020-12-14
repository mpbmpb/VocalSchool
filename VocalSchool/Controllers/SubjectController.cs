using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;

namespace VocalSchool.Controllers
{
    public class SubjectController : Controller
    {
        private readonly DbHandler _db;

        public SubjectController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            var subjects = await _db.GetAllSubjectsIncludeDaysAsync();
            return View(subjects.Where(x => x.Name[0] != '[').ToList());
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _db.GetSubjectAsync((int)id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectId,Name,Description,RequiredReading")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _db.AddAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }
        
        // GET: Subject/CreateCourseSubject
        public async Task<IActionResult> CreateCourseSubject(int id)
        {
            var course = await _db.GetCourseFullAsync(id);
            var uid = $"[{course.Name}-{id}]"; 
            return View(new CreateCourseSubjectVM(id, uid));
        }

        // POST: Subject/CreateCourseSubject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourseSubject(CreateCourseSubjectVM model)
        {
            if (ModelState.IsValid)
            {
                var subject = model.Subject;
                subject.Name = model.Subject.Name.Prepend(model.Uid);
                await _db.AddAsync(subject);
                return RedirectToAction(nameof(Edit), nameof(Course), new { id = model.CourseId});
            }
            return RedirectToAction(nameof(Index), nameof(Course));
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var subject = await _db.GetSubjectAsync((int)id);
            if (subject == null)
                return NotFound();

            string uid = subject.GetUid();
            subject.TrimUid();
            var model = new SubjectViewModel(subject, uid);
            model.LastPage = Request.GetTypedHeaders().Referer;
           
            return View(new SubjectViewModel(subject, uid));
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Uid != "")
                    model.Subject.Name = model.Subject.Name.Prepend(model.Uid);
                try
                {
                    await _db.UpdateAsync(model.Subject);
                }
                catch (Exception)
                {
                    RedirectToAction(nameof(Index));
                }
                return Redirect(model.LastPage.ToString());
            }
            return View(model); 
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _db.GetSubjectAsync((int)id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _db.GetSubjectAsync(id);
            await _db.RemoveAsync(subject);
            return RedirectToAction(nameof(Index));
        }
    }
}
