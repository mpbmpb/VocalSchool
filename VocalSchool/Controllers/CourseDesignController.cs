using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;


namespace VocalSchool.Controllers
{
    public class CourseDesignController : Controller
    {
        private readonly DbHandler _db;

        public CourseDesignController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: CourseDesign
        public async Task<IActionResult> Index()
        {
            var cd = await _db.GetAllAsync<CourseDesign>(x => x.Name.Substring(0, 1) != "[");
            return View(cd);
        }

        // GET: CourseDesign/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDesign = await _db.GetCourseDesignFullAsync(id);
            if (courseDesign == null)
            {
                return NotFound();
            }

            return View(courseDesign);
        }

        // GET: CourseDesign/Create
        public async Task<IActionResult> Create()
        {
            var seminars = await _db.GetAllSeminarsFullAsync(x => x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new CourseDesignViewModel(seminars, lastPage));
        }

        // POST: CourseDesign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDesignViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _db.AddCourseDesignAsync(model);
                return Redirect(model.LastPage);
            }
            return View(model);
        }

        // GET: CourseDesign/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDesign = await _db.GetCourseDesignFullAsync((int)id);
            if (courseDesign == null)
            {
                return NotFound();
            }
            
            List<Seminar> seminars;
            var uid = courseDesign.GetUid();
            if (uid != "")
                seminars = await _db.GetAllSeminarsFullAsync(x => 
                    x.Name.Length >= uid.Length && x.Name.Substring(0, uid.Length) == uid);
            else
                seminars = await _db.GetAllSeminarsFullAsync(x => 
                    x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";
           
            return View(new CourseDesignViewModel(courseDesign.TrimUid(), seminars, uid, lastPage));
        }

        // POST: CourseDesign/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseDesignViewModel model)
        {
            int courseDesignId = model.CourseDesign.CourseDesignId;
            if (id != courseDesignId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.Uid != "")
                    model.CourseDesign.Name = model.CourseDesign.Name.Prepend(model.Uid);
                try
                {
                    await _db.UpdateCourseDesignAsync(model);
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return Redirect(model.LastPage);
            }
            return View(model);
        }

        // GET: CourseDesign/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDesign = await _db.GetAsync<CourseDesign>(id);
            if (courseDesign == null)
            {
                return NotFound();
            }
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new CourseDesignViewModel(courseDesign, lastPage));
        }

        // POST: CourseDesign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(CourseDesignViewModel model)
        {
            var courseDesign = await _db.GetAsync<CourseDesign>(model.CourseDesign.CourseDesignId);
            await _db.RemoveCourseDesignAsync(courseDesign);
            return Redirect(model.LastPage);
        }
    }
}
