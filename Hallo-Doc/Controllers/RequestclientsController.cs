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
    public class RequestclientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestclientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requestclients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Requestclients.Include(r => r.Request);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Requestclients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Requestclients == null)
            {
                return NotFound();
            }

            var requestclient = await _context.Requestclients
                .Include(r => r.Request)
                .FirstOrDefaultAsync(m => m.RequestclientId == id);
            if (requestclient == null)
            {
                return NotFound();
            }

            return View(requestclient);
        }

        // GET: Requestclients/Create
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId");
            return View();
        }

        // POST: Requestclients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestclientId,RequestId,FirstName,LastName,PhoneNumber,Location,Address,Email,RegionId,NotiMobile,NotiEmail,Notes,StrMonth,IntYear,IntDate,IsMobile,Street,City,State,ZipCode,CommunicationType,RemindReservationCount,RemindHouseCallCount,IsSetFollowupSent,Ip,IsReservationRemindSent,Latitude,Longitude")] Requestclient requestclient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestclient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", requestclient.RequestId);
            return View(requestclient);
        }

        // GET: Requestclients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Requestclients == null)
            {
                return NotFound();
            }

            var requestclient = await _context.Requestclients.FindAsync(id);
            if (requestclient == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", requestclient.RequestId);
            return View(requestclient);
        }

        // POST: Requestclients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestclientId,RequestId,FirstName,LastName,PhoneNumber,Location,Address,Email,RegionId,NotiMobile,NotiEmail,Notes,StrMonth,IntYear,IntDate,IsMobile,Street,City,State,ZipCode,CommunicationType,RemindReservationCount,RemindHouseCallCount,IsSetFollowupSent,Ip,IsReservationRemindSent,Latitude,Longitude")] Requestclient requestclient)
        {
            if (id != requestclient.RequestclientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestclient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestclientExists(requestclient.RequestclientId))
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
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", requestclient.RequestId);
            return View(requestclient);
        }

        // GET: Requestclients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Requestclients == null)
            {
                return NotFound();
            }

            var requestclient = await _context.Requestclients
                .Include(r => r.Request)
                .FirstOrDefaultAsync(m => m.RequestclientId == id);
            if (requestclient == null)
            {
                return NotFound();
            }

            return View(requestclient);
        }

        // POST: Requestclients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Requestclients == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Requestclients'  is null.");
            }
            var requestclient = await _context.Requestclients.FindAsync(id);
            if (requestclient != null)
            {
                _context.Requestclients.Remove(requestclient);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestclientExists(int id)
        {
          return (_context.Requestclients?.Any(e => e.RequestclientId == id)).GetValueOrDefault();
        }
    }
}
