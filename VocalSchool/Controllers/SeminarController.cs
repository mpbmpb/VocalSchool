using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View(new SeminarViewModel(days));
        }

        // POST: Seminar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeminarViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _db.AddSeminarAsync(model);
                return RedirectToAction(nameof(Index));
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
            
            var days = new List<Day>();
            if (seminar.Name[0] == '[')
            {
                int length = seminar.Name.IndexOf(']') + 1;
                var uid = seminar.Name.Substring(0, length);
                days = await _db.GetAllDaysFullAsync(x => 
                    x.Name.Length >= length && x.Name.Substring(0, length) == uid);
            }
            else
            {
                days = await _db.GetAllDaysFullAsync(x => 
                    x.Name.Substring(0, 1) != "[");
            }

            return View(new SeminarViewModel(seminar, days));
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
                try
                {
                    await _db.UpdateSeminarAsync(model);
                }
                catch (Exception)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Seminar/Delete/5
        //TODO if name contains uid add message courseElements will be deleted
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

            return View(seminar);
        }

        // POST: Seminar/Delete/5
        //TODO if name contains uid delete all courseElements & Many2Many relations as well
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminar = await _db.GetSeminarAndDaysAsync((int)id);
            await _db.RemoveAsync(seminar);
            return RedirectToAction(nameof(Index));
        }
    }
}
