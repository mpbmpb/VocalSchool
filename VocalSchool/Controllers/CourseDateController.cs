using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VocalSchool.Data;
using VocalSchool.Models;

namespace VocalSchool.Controllers
{
    public class CourseDateController : Controller
    {
        private readonly SchoolContext _context;

        public CourseDateController(SchoolContext context)
        {
            _context = context;
        }

        // GET: CourseDate
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.CourseDates.Include(c => c.Course).Include(c => c.Venue);
            return View(await schoolContext.ToListAsync());
        }

        // GET: CourseDate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDate = await _context.CourseDates
                .Include(c => c.Course)
                .Include(c => c.Venue)
                .FirstOrDefaultAsync(m => m.CourseDateId == id);
            if (courseDate == null)
            {
                return NotFound();
            }

            return View(courseDate);
        }

        // GET: CourseDate/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Name");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");
            return View();
        }

        // POST: CourseDate/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseDateId,Date,VenueId,ReservationInfo,Rider,CourseId")] CourseDate courseDate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", courseDate.CourseId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", courseDate.VenueId);
            return View(courseDate);
        }

        // GET: CourseDate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDate = await _context.CourseDates.FindAsync(id);
            if (courseDate == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", courseDate.CourseId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", courseDate.VenueId);
            return View(courseDate);
        }

        // POST: CourseDate/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseDateId,Date,VenueId,ReservationInfo,Rider,CourseId")] CourseDate courseDate)
        {
            if (id != courseDate.CourseDateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseDate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseDateExists(courseDate.CourseDateId))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", courseDate.CourseId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", courseDate.VenueId);
            return View(courseDate);
        }

        // GET: CourseDate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDate = await _context.CourseDates
                .Include(c => c.Course)
                .Include(c => c.Venue)
                .FirstOrDefaultAsync(m => m.CourseDateId == id);
            if (courseDate == null)
            {
                return NotFound();
            }

            return View(courseDate);
        }

        // POST: CourseDate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseDate = await _context.CourseDates.FindAsync(id);
            _context.CourseDates.Remove(courseDate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseDateExists(int id)
        {
            return _context.CourseDates.Any(e => e.CourseDateId == id);
        }
    }
}
