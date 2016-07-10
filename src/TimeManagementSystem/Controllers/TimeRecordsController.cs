using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace TimeManagementSystem.Controllers
{
    [Authorize(Roles = "Standard")]
    public class TimeRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeRecordsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: TimeRecords
        public async Task<IActionResult> Index()
        {
            string currentUser = this.User.Identity.Name;
            List<TimeRecord> loggedRecords = await _context.TimeRecord.Where(x => x.UserName == currentUser).ToListAsync();
            loggedRecords = loggedRecords.OrderByDescending(x => x.RecordDate).ToList();
            return View(loggedRecords);
        }

        // GET: TimeRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id);
            if (timeRecord == null)
            {
                return NotFound();
            }

            return View(timeRecord);
        }

        // GET: TimeRecords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimeRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Comments,DurationWork,RecordDate,TimeBreak,TimeWorkEnd,TimeWorkStart")] TimeRecord timeRecord)
        {
            if (ModelState.IsValid)
            {
                CalculateTime(ref timeRecord);

                _context.Add(timeRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(timeRecord);
        }
        

        // GET: TimeRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id);
            if (timeRecord == null)
            {
                return NotFound();
            }
            return View(timeRecord);
        }

        // POST: TimeRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Comments,DurationWork,RecordDate,TimeBreak,TimeWorkEnd,TimeWorkStart")] TimeRecord timeRecord)
        {
            if (id != timeRecord.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CalculateTime(ref timeRecord);

                    _context.Update(timeRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeRecordExists(timeRecord.ID))
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
            return View(timeRecord);
        }

        // GET: TimeRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id);
            if (timeRecord == null)
            {
                return NotFound();
            }

            return View(timeRecord);
        }

        // POST: TimeRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id);
            _context.TimeRecord.Remove(timeRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TimeRecordExists(int id)
        {
            return _context.TimeRecord.Any(e => e.ID == id);
        }

        private void CalculateTime(ref TimeRecord timeRecord)
        {
            timeRecord.UserName = this.User.Identity.Name;

            timeRecord.TimeWorkStart = new DateTime(timeRecord.RecordDate.Year,
                                                    timeRecord.RecordDate.Month,
                                                    timeRecord.RecordDate.Day,
                                                    timeRecord.TimeWorkStart.Hour,
                                                    timeRecord.TimeWorkStart.Minute,
                                                    timeRecord.TimeWorkStart.Second);

            if (timeRecord.TimeWorkEnd != null)
            {
                timeRecord.TimeWorkEnd = new DateTime(timeRecord.RecordDate.Year,
                                            timeRecord.RecordDate.Month,
                                            timeRecord.RecordDate.Day,
                                            ((DateTime)timeRecord.TimeWorkEnd).Hour,
                                            ((DateTime)timeRecord.TimeWorkEnd).Minute,
                                            ((DateTime)timeRecord.TimeWorkEnd).Second);

                if (timeRecord.TimeWorkEnd > timeRecord.TimeWorkStart)
                {
                    timeRecord.DurationWork = (DateTime)timeRecord.TimeWorkEnd - timeRecord.TimeWorkStart;
                }

                if (timeRecord.TimeBreak != null)
                {
                    if (timeRecord.TimeBreak <= timeRecord.DurationWork.TotalMinutes)
                    {
                        timeRecord.DurationWork = timeRecord.DurationWork - TimeSpan.FromMinutes((double)timeRecord.TimeBreak);
                    }
                }
            }
        }
    }
}
