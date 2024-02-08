using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Data;
using Hallo_Doc.Models;
using System.Security.AccessControl;

namespace Hallo_Doc.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users.Include(u => u.Aspnetuser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Aspnetuser)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Aspnetuserid,Firstname,Lastname,Email,Mobile,Ismobile,Street,City,State,Regionid,Zipcode,Strmonth,Stryear,Intdate,Createdby,Createddate,Modifiedby,Modifieddate,Status,Isdeleted,Ip,Isrequestwithemail")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id", user.Aspnetuserid);
            return View(user);
        }

        public IActionResult Create_patient_req()
        {
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_patient_req([Bind("Userid,Aspnetuserid,Firstname,Lastname,Email,Mobile,Ismobile,Street,City,State,Regionid,Zipcode,Strmonth,Stryear,Intdate,Createdby,Createddate,Modifiedby,Modifieddate,Status,Isdeleted,Ip,Isrequestwithemail")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ViewData["Error"] = "An error occured while saving the data.";
                    return View("Create_request", user);
                }
            }
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id", user.Aspnetuserid);
            return View("Create_patient_request", user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id", user.Aspnetuserid);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Userid,Aspnetuserid,Firstname,Lastname,Email,Mobile,Ismobile,Street,City,State,Regionid,Zipcode,Strmonth,Stryear,Intdate,Createdby,Createddate,Modifiedby,Modifieddate,Status,Isdeleted,Ip,Isrequestwithemail")] User user)
        {
            if (id != user.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Userid))
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
            ViewData["Aspnetuserid"] = new SelectList(_context.AspnetUsers, "Id", "Id", user.Aspnetuserid);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Aspnetuser)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
}
