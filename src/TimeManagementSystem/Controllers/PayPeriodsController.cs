using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace TimeManagementSystem.Controllers
{
    [Authorize(Roles = "Standard, Manager")]
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
            string currentUser = this.User.Identity.Name;
            List<PayPeriod> payPeriodList = await _context.PayPeriod.Where(x => x.UserName == currentUser).ToListAsync();
            payPeriodList = payPeriodList.OrderByDescending(x => x.PeriodEnd).ToList();
            return View(payPeriodList);
        }

        // GET: PayPeriods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (payPeriod == null)
            {
                return await CannotAccessModify();
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
                // Check that the end date is after the start date 
                if (payPeriod.PeriodEnd < payPeriod.PeriodStart)
                {
                    ModelState.AddModelError(string.Empty, "Pay period end date cannot be before start date.");
                    return View();
                }

                CalculatePayPeriod(ref payPeriod);

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
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (payPeriod == null)
            {
                return await CannotAccessModify();
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
                    // Check that the end date is after the start date 
                    if (payPeriod.PeriodEnd < payPeriod.PeriodStart)
                    {
                        ModelState.AddModelError(string.Empty, "Pay period end date cannot be before start date.");
                        return View();
                    }

                    CalculatePayPeriod(ref payPeriod);

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
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var payPeriod = await _context.PayPeriod.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (payPeriod == null)
            {
                return await CannotAccessModify();
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
        private void CalculatePayPeriod(ref PayPeriod payPeriod)
        {
            string currentUser = this.User.Identity.Name;
            payPeriod.UserName = currentUser;

            List<TimeRecord> timeRecords = _context.TimeRecord.ToList();
            DateTime periodStart = payPeriod.PeriodStart;
            DateTime periodEnd = payPeriod.PeriodEnd;
            timeRecords = timeRecords.Where(x => x.UserName == currentUser &&
                                                 x.RecordDate >= periodStart &&
                                                 x.RecordDate <= periodEnd).ToList();

            TimeSpan totalTimeWorked = TimeSpan.Zero;
            foreach (var timeRecordItem in timeRecords)
            {
                totalTimeWorked += timeRecordItem.DurationWork;
            }

            payPeriod.PeriodTime = totalTimeWorked;
            payPeriod.PeriodTotalTime = payPeriod.PeriodTime;

            if (payPeriod.MiscMin != null)
            {
                payPeriod.PeriodTotalTime += TimeSpan.FromMinutes((int)payPeriod.MiscMin);
            }
            else
            {
                payPeriod.MiscMin = 0;
            }
        }

        private async Task<IActionResult> CannotAccessModify()
        {
            ModelState.AddModelError(string.Empty, "Cannot find the specified pay period.");
            string currentUser = this.User.Identity.Name;
            List<PayPeriod> loggedRecords = await _context.PayPeriod.Where(x => x.UserName == currentUser).ToListAsync();
            loggedRecords = loggedRecords.OrderByDescending(x => x.PeriodEnd).ToList();
            return View("Index", loggedRecords);
        }
    }
}
