using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;

namespace VocalSchool.Controllers
{
    public class VenueController : Controller
    {
        private readonly DbHandler _db;

        public VenueController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetAllAsync<Venue>());
        }

        // GET: Venue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _db.GetAsync<Venue>(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venue/Create
        public async Task<IActionResult> Create()
        {
            var contacts = await _db.GetAllAsync<Contact>();
            return View(new VenueViewModel(contacts));
        }

        // POST: Venue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _db.AddVenueAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Venue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _db.GetAsync<Venue>(id);
            if (venue == null)
            {
                return NotFound();
            }
            var contacts = await _db.GetAllAsync<Contact>();
            return View(new VenueViewModel(venue, contacts));
        }

        // POST: Venue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VenueViewModel model)
        {
            if (id != model.Venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _db.UpdateVenueAsync(model);
                }
                catch (Exception)
                {
                    RedirectToAction(nameof(Index));
                } 
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Venue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _db.GetAsync<Venue>(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _db.GetAsync<Venue>(id);
            await _db.RemoveAsync(venue);
            return RedirectToAction(nameof(Index));
        }
    }
}
