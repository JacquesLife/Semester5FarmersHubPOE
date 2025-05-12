using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Programming3A.Database;
using Programming3A.Models;

namespace Programming3A.Controllers
{
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Farmer
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Farmers.Include(f => f.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Farmer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmerModel = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FarmerId == id);
            if (farmerModel == null)
            {
                return NotFound();
            }

            return View(farmerModel);
        }

        // GET: Farmer/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Farmer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FarmerId,UserId,FullName,PhoneNumber")] FarmerModel farmerModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(farmerModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", farmerModel.UserId);
            return View(farmerModel);
        }

        // GET: Farmer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmerModel = await _context.Farmers.FindAsync(id);
            if (farmerModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", farmerModel.UserId);
            return View(farmerModel);
        }

        // POST: Farmer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FarmerId,UserId,FullName,PhoneNumber")] FarmerModel farmerModel)
        {
            if (id != farmerModel.FarmerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerModelExists(farmerModel.FarmerId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", farmerModel.UserId);
            return View(farmerModel);
        }

        // GET: Farmer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmerModel = await _context.Farmers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FarmerId == id);
            if (farmerModel == null)
            {
                return NotFound();
            }

            return View(farmerModel);
        }

        // POST: Farmer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmerModel = await _context.Farmers.FindAsync(id);
            if (farmerModel != null)
            {
                _context.Farmers.Remove(farmerModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FarmerModelExists(int id)
        {
            return _context.Farmers.Any(e => e.FarmerId == id);
        }
    }
}
