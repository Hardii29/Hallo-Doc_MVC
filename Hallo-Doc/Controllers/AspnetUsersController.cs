using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Data;
using Hallo_Doc.Models;

namespace Hallo_Doc.Controllers
{
    public class AspnetUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AspnetUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AspnetUsers
        public async Task<IActionResult> Index()
        {
              return _context.AspnetUsers != null ? 
                          View(await _context.AspnetUsers.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.AspnetUsers'  is null.");
        }

        // GET: AspnetUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.AspnetUsers == null)
            {
                return NotFound();
            }

            var aspnetUser = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetUser == null)
            {
                return NotFound();
            }

            return View(aspnetUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Patient_dashboard(string email, string password)
        {
            if (email == null || password == null)
            {
                return NotFound("Email and Password Required.");
            }

            var User = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Email == email && m.Passwordhash == password);
            if (User == null)
            {
                return NotFound();
            }

            return RedirectToAction("Patient_dashboard", "Patient");
        }

        // GET: AspnetUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Passwordhash,Email,Phonenumber")] AspnetUser aspnetUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspnetUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aspnetUser);
        }

        // GET: AspnetUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AspnetUsers == null)
            {
                return NotFound();
            }

            var aspnetUser = await _context.AspnetUsers.FindAsync(id);
            if (aspnetUser == null)
            {
                return NotFound();
            }
            return View(aspnetUser);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Username,Passwordhash,Securitystamp,Email,Emailconfirmed,Phonenumber,Phonenumberconfirmed,Twofactorenabled")] AspnetUser aspnetUser)
        {
            if (id != aspnetUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspnetUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspnetUserExists(aspnetUser.Id))
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
            return View(aspnetUser);
        }

        // GET: AspnetUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AspnetUsers == null)
            {
                return NotFound();
            }

            var aspnetUser = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetUser == null)
            {
                return NotFound();
            }

            return View(aspnetUser);
        }

        // POST: AspnetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AspnetUsers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AspnetUsers'  is null.");
            }
            var aspnetUser = await _context.AspnetUsers.FindAsync(id);
            if (aspnetUser != null)
            {
                _context.AspnetUsers.Remove(aspnetUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspnetUserExists(string id)
        {
          return (_context.AspnetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
