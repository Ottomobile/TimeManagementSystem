using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeManagementSystem.Models;
using TimeManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


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
    }
}
