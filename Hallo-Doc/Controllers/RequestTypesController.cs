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
    public class RequestTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RequestTypes
        public async Task<IActionResult> Index()
        {
              return _context.RequestTypes != null ? 
                          View(await _context.RequestTypes.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.RequestTypes'  is null.");
        }

        // GET: RequestTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RequestTypes == null)
            {
                return NotFound();
            }

            var requestType = await _context.RequestTypes
                .FirstOrDefaultAsync(m => m.RequestTypeId == id);
            if (requestType == null)
            {
                return NotFound();
            }

            return View(requestType);
        }

        // GET: RequestTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RequestTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestTypeId,Name")] RequestType requestType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestType);
        }

        // GET: RequestTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RequestTypes == null)
            {
                return NotFound();
            }

            var requestType = await _context.RequestTypes.FindAsync(id);
            if (requestType == null)
            {
                return NotFound();
            }
            return View(requestType);
        }

        // POST: RequestTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestTypeId,Name")] RequestType requestType)
        {
            if (id != requestType.RequestTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestTypeExists(requestType.RequestTypeId))
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
            return View(requestType);
        }

        // GET: RequestTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RequestTypes == null)
            {
                return NotFound();
            }

            var requestType = await _context.RequestTypes
                .FirstOrDefaultAsync(m => m.RequestTypeId == id);
            if (requestType == null)
            {
                return NotFound();
            }

            return View(requestType);
        }

        // POST: RequestTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RequestTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.RequestTypes'  is null.");
            }
            var requestType = await _context.RequestTypes.FindAsync(id);
            if (requestType != null)
            {
                _context.RequestTypes.Remove(requestType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestTypeExists(int id)
        {
          return (_context.RequestTypes?.Any(e => e.RequestTypeId == id)).GetValueOrDefault();
        }
    }
}
