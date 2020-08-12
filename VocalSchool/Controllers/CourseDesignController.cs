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
    public class CourseDesignController : Controller
    {
        private readonly SchoolContext _context;
        private readonly DbHandler _db;

        public CourseDesignController(SchoolContext context)
        {
            _context = context;
            _db = new DbHandler(context);
        }

        // GET: CourseDesign
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetAllCourseDesignsAsync());
        }

        // GET: CourseDesign/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDesign = await _db.GetCourseDesignIncludingSubjectsAsync((int)id);
            if (courseDesign == null)
            {
                return NotFound();
            }

            return View(courseDesign);
        }

        // GET: CourseDesign/Create
        public async Task<IActionResult> Create()
        {
            var seminars = await _db.GetAllSeminarsAsync();
            return View(new CourseDesignViewModel(seminars));
        }

        // POST: CourseDesign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDesignViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _db.AddCourseDesignAsync(model);
                return RedirectToAction(nameof(Index));
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

            var courseDesign = await _db.GetCourseDesignIncludingSubjectsAsync((int)id);
            if (courseDesign == null)
            {
                return NotFound();
            }

            var seminars = await _db.GetAllSeminarsAsync();
            return View(new CourseDesignViewModel(courseDesign, seminars));
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
                try
                {
                    await _db.UpdateCourseDesignAsync(model);
                }
                catch (Exception)
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
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

            var courseDesign = await _context.CourseDesigns
                .FirstOrDefaultAsync(m => m.CourseDesignId == id);
            if (courseDesign == null)
            {
                return NotFound();
            }

            return View(courseDesign);
        }

        // POST: CourseDesign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseDesign = await _context.CourseDesigns.FindAsync(id);
            _context.CourseDesigns.Remove(courseDesign);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseDesignExists(int id)
        {
            return _context.CourseDesigns.Any(e => e.CourseDesignId == id);
        }
    }
}
