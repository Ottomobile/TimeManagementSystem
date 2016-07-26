using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeManagementSystem.Models;
using TimeManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace TimeManagementSystem.Views.SubscribeToUsers
{
    [Authorize(Roles = "Manager")]
    public class ManageUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageUsers
        public async Task<IActionResult> Index()
        {
            string currentUser = this.User.Identity.Name;
            List<SubscribeToUser> subscribedUsersList = await _context.SubscribeToUser.Where(x => x.ManagingUser == currentUser).ToListAsync();
            return View(subscribedUsersList);
        }

        // GET: ManageUsers/ViewTimeRecords/<managedUser>
        public async Task<IActionResult> ViewTimeRecords(string managedUser)
        {
            ViewData["ManagedUser"] = managedUser;
            List<TimeRecord> loggedRecords = await _context.TimeRecord.Where(x => x.UserName == managedUser).ToListAsync();
            loggedRecords = loggedRecords.OrderByDescending(x => x.RecordDate).ToList();
            return View(loggedRecords);
        }

        // GET: ManageUsers/ViewPayPeriods/<managedUser>
        public async Task<IActionResult> ViewPayPeriods(string managedUser)
        {
            ViewData["ManagedUser"] = managedUser;
            List<PayPeriod> payPeriodList = await _context.PayPeriod.Where(x => x.UserName == managedUser).ToListAsync();
            payPeriodList = payPeriodList.OrderByDescending(x => x.PeriodEnd).ToList();
            return View(payPeriodList);
        }

        /*
         * Writes the time records of the managed user to a CSV file and
         * forces the browser to download this file.
         */
        [HttpPost]
        public void ExportToCSV()
        {
            Response.Clear();
            Response.Headers.Add("content-disposition", "attachment; filename=testfile.txt");
            Response.Headers.Add("content-type", "text/plain");
            
            using (StreamWriter writer = new StreamWriter(Response.Body))
            {
                writer.WriteLine("Date,Start Time,End Time,Break (Min),Duration Worked,Comments");

                string managedUser = this.Request.Form["ExportUser"];
                List<TimeRecord> timeRecordsList = _context.TimeRecord.Where(x => x.UserName == managedUser).ToList<TimeRecord>();
                timeRecordsList = timeRecordsList.OrderByDescending(x => x.RecordDate).ToList();

                foreach (var timeRecord in timeRecordsList)
                {
                    string RecordDate = (timeRecord.RecordDate != null) ? timeRecord.RecordDate.ToString("MM/dd/yyyy") : "";
                    string TimeWorkStart = (timeRecord.TimeWorkStart != null) ? timeRecord.TimeWorkStart.ToString("hh:mm tt") : "";
                    string TimeWorkEnd = (timeRecord.TimeWorkEnd != null) ? ((DateTime)(timeRecord.TimeWorkEnd)).ToString("hh:mm tt") : "";
                    string TimeBreak = (timeRecord.TimeBreak != null) ? timeRecord.TimeBreak.ToString() : "";
                    string DurationWork = (timeRecord.DurationWork != null) ? timeRecord.DurationWork.ToString() : "";
                    string Comments = (timeRecord.Comments != null) ? timeRecord.Comments.ToString() : "";

                    writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                                    RecordDate, TimeWorkStart, TimeWorkEnd,
                                    TimeBreak, DurationWork, Comments);
                }
            }
        }
    }
}
