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
            return View(await _context.TimeRecord.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("ID,Comments,DurationWork,RecordDate,TimeBreak,TimeTotal,TimeWorkEnd,TimeWorkStart")] TimeRecord timeRecord)
        {
            if (ModelState.IsValid)
            {
                timeRecord.TimeWorkStart = new DateTime(timeRecord.RecordDate.Year,
                                                    timeRecord.RecordDate.Month,
                                                    timeRecord.RecordDate.Day,
                                                    timeRecord.TimeWorkStart.Hour,
                                                    timeRecord.TimeWorkStart.Minute,
                                                    timeRecord.TimeWorkStart.Second);

                timeRecord.TimeWorkEnd = new DateTime(timeRecord.RecordDate.Year,
                                            timeRecord.RecordDate.Month,
                                            timeRecord.RecordDate.Day,
                                            timeRecord.TimeWorkEnd.Hour,
                                            timeRecord.TimeWorkEnd.Minute,
                                            timeRecord.TimeWorkEnd.Second);

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Comments,DurationBreak,DurationWork,RecordDate,TimeBreakEnd,TimeBreakStart,TimeTotal,TimeWorkEnd,TimeWorkStart")] TimeRecord timeRecord)
        {
            if (id != timeRecord.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
    }
}
