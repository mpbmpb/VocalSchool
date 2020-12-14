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
    public class SeminarController : Controller
    {
        private readonly DbHandler _db;

        public SeminarController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: Seminar
        public async Task<IActionResult> Index()
        {
            var seminars = await _db.GetAllSeminarsFullAsync(x => x.Name.Substring(0, 1) != "[");
            return View(seminars);
        }

        // GET: Seminar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Seminar seminar = await _db.GetSeminarFullAsync(id);

            if (seminar == null)
            {
                return NotFound();
            }
            return View(seminar);
        }

        // GET: Seminar/Create
        public async Task<IActionResult> Create()
        {
            var days = await _db.GetAllDaysFullAsync(x => x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";
            return View(new SeminarViewModel(days, lastPage));
        }

        // POST: Seminar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeminarViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _db.AddSeminarAsync(model);
                return Redirect(model.LastPage);
            }
            return View(model);
        }

        // GET: Seminar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _db.GetSeminarAndDaysAsync((int)id);
            if (seminar == null)
            {
                return NotFound();
            }
            
            List<Day> days;
            var uid = seminar.GetUid();
            if (uid != "")
                days = await _db.GetAllDaysFullAsync(x => 
                    x.Name.Length >= uid.Length && x.Name.Substring(0, uid.Length) == uid);
            else
                days = await _db.GetAllDaysFullAsync(x => 
                    x.Name.Substring(0, 1) != "[");
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new SeminarViewModel(seminar.TrimUid(), days, uid, lastPage));
        }

        // POST: Seminar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeminarViewModel model)
        {
            int seminarId = model.Seminar.SeminarId;
            if (id != seminarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.Uid != "")
                    model.Seminar.Name = model.Seminar.Name.Prepend(model.Uid);
                try
                {
                    await _db.UpdateSeminarAsync(model);
                }
                catch (Exception)
                {
                    return RedirectToAction(nameof(Index));
                }

                return Redirect(model.LastPage);
            }
            return View(model);
        }

        // GET: Seminar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _db.GetSeminarAndDaysAsync((int)id);
            if (seminar == null)
            {
                return NotFound();
            }
            var lastPage = Request?.GetTypedHeaders()?.Referer?.ToString() ?? "http://completevocaltraining.nl";

            return View(new SeminarViewModel(seminar, lastPage));
        }

        // POST: Seminar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(SeminarViewModel model)
        {
            var seminar = await _db.GetSeminarAndDaysAsync(model.Seminar.SeminarId);
            await _db.RemoveAsync(seminar);
            return Redirect(model.LastPage);
        }
    }
}
