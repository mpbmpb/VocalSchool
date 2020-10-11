using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;

namespace VocalSchool.Controllers
{
    public class DayController : Controller
    {
        private readonly DbHandler _db;

        public DayController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: Day
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetAllAsync<Day>());
        }

        // GET: Day/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _db.GetAsync<Day>(id);
            if (day == null)
            {
                return NotFound();
            }

            return View(day);
        }

        // GET: Day/Create
        public async Task<IActionResult> Create()
        {
            var subjects = await _db.GetAllAsync<Subject>();

            return View(new DayViewModel(subjects));
        }

        // POST: Day/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DayViewModel model)
        {

            if (ModelState.IsValid)
            {
                await _db.AddDayAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Day/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _db.GetDayFullAsync(id);

            if (day == null)
            {
                return NotFound();
            }
            var subjects = await _db.GetAllAsync<Subject>();

            return View(new DayViewModel(day, subjects));
        }

        // POST: Day/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DayViewModel model)
        {
            int dayId = model.Day.DayId;

            if (id != dayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _db.UpdateDayAsync(model);
                }
                catch (Exception)
                {
                    RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Day/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var day = await _db.GetAsync<Day>(id);
            if (day == null)
            {
                return NotFound();
            }

            return View(day);
        }

        // POST: Day/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var day = await _db.GetAsync<Day>(id);

            await _db.RemoveAsync(day);
            return RedirectToAction(nameof(Index));
        }

        public ControllerContext GetContext([FromServices] ControllerContext ctx)
        {
            var c = ctx;
            return c;
        }
    }
}
