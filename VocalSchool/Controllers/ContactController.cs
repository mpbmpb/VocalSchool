using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocalSchool.Models;
using VocalSchool.Data;

namespace VocalSchool.Controllers
{
    public class ContactController : Controller
    {
        private readonly DbHandler _db;

        public ContactController(SchoolContext context)
        {
            _db = new DbHandler(context);
        }

        // GET: Contact
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetAllAsync<Contact>());
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _db.GetAsync<Contact>(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,Name,Email,Phone,Adress")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                await _db.AddAsync(contact);
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _db.GetAsync<Contact>(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,Name,Email,Phone,Adress")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _db.UpdateAsync(contact);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.ContactExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(contact);
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _db.GetAsync<Contact>(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _db.GetAsync<Contact>(id);
            await _db.RemoveAsync(contact);
            return RedirectToAction(nameof(Index));
        }
    }
}
