using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;

namespace TimeManagementSystem.Controllers
{
    public class PayPeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayPeriodsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: PayPeriods
        public async Task<IActionResult> Index()
        {
            return View(await _context.PayPeriod.ToListAsync());
        }

        // GET: PayPeriods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id);
            if (payPeriod == null)
            {
                return NotFound();
            }

            return View(payPeriod);
        }

        // GET: PayPeriods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayPeriods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Comments,PeriodEnd,PeriodStart,PeriodTime,MiscMin,PeriodTotalTime")] PayPeriod payPeriod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payPeriod);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(payPeriod);
        }

        // GET: PayPeriods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id);
            if (payPeriod == null)
            {
                return NotFound();
            }
            return View(payPeriod);
        }

        // POST: PayPeriods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Comments,PeriodEnd,PeriodStart,PeriodTime,MiscMin,PeriodTotalTime")] PayPeriod payPeriod)
        {
            if (id != payPeriod.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payPeriod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayPeriodExists(payPeriod.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(payPeriod);
        }

        // GET: PayPeriods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id);
            if (payPeriod == null)
            {
                return NotFound();
            }

            return View(payPeriod);
        }

        // POST: PayPeriods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id);
            _context.PayPeriod.Remove(payPeriod);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PayPeriodExists(int id)
        {
            return _context.PayPeriod.Any(e => e.ID == id);
        }
    }
}
