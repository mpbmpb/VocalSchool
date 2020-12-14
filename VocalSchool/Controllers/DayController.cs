using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            var days = await _db.GetAllAsync<Day>();
            return View(days.Where(x => x.Name[0] != '[').ToList());
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
            var subjects = await _db.GetAllAsync<Subject>(x => x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new DayViewModel(subjects, lastPage));
        }

        // POST: Day/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DayViewModel model)
        {

            if (ModelState.IsValid)
            {
                await _db.AddDayAsync(model);
                return Redirect(model.LastPage);
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

            List<Subject> subjects;
            var uid = day.GetUid();
            if (uid != "")
                subjects = await _db.GetAllAsync<Subject>(x => 
                    x.Name.Length >= uid.Length && x.Name.Substring(0, uid.Length) == uid);
            else
                subjects = await _db.GetAllAsync<Subject>(x => 
                    x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new DayViewModel(day.TrimUid(), subjects, uid, lastPage));
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
                if (model.Uid != "")
                    model.Day.Name = model.Day.Name.Prepend(model.Uid);
                try
                {
                    await _db.UpdateDayAsync(model);
                }
                catch (Exception)
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect(model.LastPage);
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
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new DayViewModel(day, lastPage));
        }

        // POST: Day/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DayViewModel model)
        {
            var day = await _db.GetAsync<Day>(model.Day.DayId);

            await _db.RemoveAsync(day);
            return Redirect(model.LastPage);
        }

        public ControllerContext GetContext([FromServices] ControllerContext ctx)
        {
            var c = ctx;
            return c;
        }
    }
}
