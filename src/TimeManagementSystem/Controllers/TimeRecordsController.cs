using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace TimeManagementSystem.Controllers
{
    [Authorize(Roles = "Standard, Manager")]
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
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (timeRecord == null)
            {
                return await CannotAccessModify();
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
                bool result = CalculateTime(ref timeRecord);

                if (result)
                {
                    _context.Add(timeRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(timeRecord);
        }
        

        // GET: TimeRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (timeRecord == null)
            {
                return await CannotAccessModify();
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
                    bool result = CalculateTime(ref timeRecord);

                    if (result)
                    {
                        _context.Update(timeRecord);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return View(timeRecord);
                    }
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
                return await CannotAccessModify();
            }

            string currentUser = this.User.Identity.Name;
            var timeRecord = await _context.TimeRecord.SingleOrDefaultAsync(m => m.ID == id && m.UserName == currentUser);
            if (timeRecord == null)
            {
                return await CannotAccessModify();
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

        /*
         * Calculates the total time work for one time record.
         * Also validates if the entered times are valid.
         * 
         * Param: TimeRecord
         *          Required: RecordDate, TimeWorkStart
         *          Optional: TimeWorkEnd, BreakTime, Comments
         * Return: true if time was calculated successfully,
         *         false if there is an error with one of the fields
         */         
        private bool CalculateTime(ref TimeRecord timeRecord)
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

                if (timeRecord.TimeWorkEnd >= timeRecord.TimeWorkStart)
                {
                    timeRecord.DurationWork = (DateTime)timeRecord.TimeWorkEnd - timeRecord.TimeWorkStart;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The end time cannot be less than the start time.");
                    return false;
                }

                if (timeRecord.TimeBreak != null)
                {
                    if (timeRecord.TimeBreak <= timeRecord.DurationWork.TotalMinutes)
                    {
                        timeRecord.DurationWork = timeRecord.DurationWork - TimeSpan.FromMinutes((double)timeRecord.TimeBreak);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The break time cannot exceed the duration worked.");
                        return false;
                    }
                }
            }
            return true;
        }

        /*
         * Returns the logged in user to the index page if the specified record
         * cannot be accessed.
         * This is done to ensure that users manipulating the URL cannot
         * access the records of others that they should not have access to.
         * 
         * Return: Index view containing error
         */  
        private async Task<IActionResult> CannotAccessModify()
        {
            ModelState.AddModelError(string.Empty, "Cannot find the specified time record.");
            string currentUser = this.User.Identity.Name;
            List<TimeRecord> loggedRecords = await _context.TimeRecord.Where(x => x.UserName == currentUser).ToListAsync();
            loggedRecords = loggedRecords.OrderByDescending(x => x.RecordDate).ToList();
            return View("Index", loggedRecords);
        }
        
        /*
         * Writes the time records of the specified user to a CSV file and
         * forces the browser to download this file.
         */
        [HttpPost]
        public void ExportTimeRecordsToCSV()
        {
            string currentUser = this.User.Identity.Name;
            Response.Clear();
            Response.Headers.Add("content-disposition", String.Format("attachment; filename=TimeRecords - {0}.csv",currentUser));
            Response.Headers.Add("content-type", "text/plain");

            using (StreamWriter writer = new StreamWriter(Response.Body))
            {
                writer.WriteLine("Date,Start Time,End Time,Break (Min),Duration Worked,Comments");
                
                List<TimeRecord> timeRecordsList = _context.TimeRecord.Where(x => x.UserName == currentUser).ToList<TimeRecord>();
                timeRecordsList = timeRecordsList.OrderByDescending(x => x.RecordDate).ToList();

                foreach (var timeRecord in timeRecordsList)
                {
                    string RecordDate = (timeRecord.RecordDate != null) ? timeRecord.RecordDate.ToString("MM/dd/yyyy") : "";
                    string TimeWorkStart = (timeRecord.TimeWorkStart != null) ? timeRecord.TimeWorkStart.ToString("hh:mm tt") : "";
                    string TimeWorkEnd = (timeRecord.TimeWorkEnd != null) ? ((DateTime)(timeRecord.TimeWorkEnd)).ToString("hh:mm tt") : "";
                    string TimeBreak = (timeRecord.TimeBreak != null) ? timeRecord.TimeBreak.ToString() : "";
                    string DurationWork = (timeRecord.DurationWork != null) ? timeRecord.DurationWork.ToString() : "";
                    string Comments = (timeRecord.Comments != null) ? "\"" + timeRecord.Comments.ToString() + "\"" : "";
                    
                    writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                                    RecordDate, TimeWorkStart, TimeWorkEnd,
                                    TimeBreak, DurationWork, Comments);
                }
            }
        }
    }
}
