using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace TimeManagementSystem.Controllers
{
    [Authorize(Roles = "Standard, Manager")]
    public class SubscribeToUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscribeToUsersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: SubscribeToUsers
        public async Task<IActionResult> Index()
        {
            string currentUser = this.User.Identity.Name;
            List<SubscribeToUser> subscribedUsersList = await _context.SubscribeToUser.Where(x => x.CurrentUser == currentUser).ToListAsync();
            return View(subscribedUsersList);
        }

        // GET: SubscribeToUsers/Add
        public IActionResult Add()
        {
            return View();
        }

        // GET: SubscribeToUsers/AddInitial
        public IActionResult AddInitial()
        {
            return View();
        }

        // POST: SubscribeToUsers/Add
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("ID,CurrentUser,ManagingUser")] SubscribeToUser subscribeToUser)
        {
            if (ModelState.IsValid)
            {
                string currentUser = this.User.Identity.Name;
                subscribeToUser.CurrentUser = currentUser;

                // Check that a user cannot add themselves as a manager
                if (subscribeToUser.ManagingUser == currentUser)
                {
                    ModelState.AddModelError(string.Empty, "Managing user cannot be the same as the current user.");
                    return View(subscribeToUser);
                }

                // Check if the manager has already been added
                string managingUser = subscribeToUser.ManagingUser;
                if( _context.SubscribeToUser.Where(x => x.CurrentUser == currentUser &&          
                                                        x.ManagingUser == managingUser).ToList().Count > 0)
                {
                    ModelState.AddModelError(string.Empty, "Manager already added previously.");
                    return View(subscribeToUser);
                }

                // Determine if the managing user exists and is a manager
                bool isManagingUser = false;
                var connection = _context.Database.GetDbConnection();
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM [dbo].[ManagersView] WHERE [ManagerUserName] = @managerUserName";
                    IDbDataParameter managerParam = command.CreateParameter();
                    managerParam.ParameterName = "@managerUserName";
                    managerParam.Value = subscribeToUser.ManagingUser;
                    command.Parameters.Add(managerParam);
                    var result = command.ExecuteReader();
                    isManagingUser = result.HasRows;
                }
                connection.Close();

                if (isManagingUser) {
                    _context.Database.GetDbConnection().Open();
                    _context.Add(subscribeToUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User entered is not a manager.");
                }
            }
            return View(subscribeToUser);
        }

        // GET: SubscribeToUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string currentUser = this.User.Identity.Name;

            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot find the specified user to delete.");
                List<SubscribeToUser> subscribedUsersList = await _context.SubscribeToUser.Where(x => x.CurrentUser == currentUser || x.ManagingUser == currentUser).ToListAsync();
                return View("Index", subscribedUsersList);
            }

            var subscribeToUser = await _context.SubscribeToUser.SingleOrDefaultAsync(m => m.ID == id && m.CurrentUser == currentUser || m.ManagingUser == currentUser);
            if (subscribeToUser == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot find the specified user to delete.");
                List<SubscribeToUser> subscribedUsersList = await _context.SubscribeToUser.Where(x => x.CurrentUser == currentUser || x.ManagingUser == currentUser).ToListAsync();
                return View("Index", subscribedUsersList);
            }

            return View(subscribeToUser);
        }

        // POST: SubscribeToUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscribeToUser = await _context.SubscribeToUser.SingleOrDefaultAsync(m => m.ID == id);
            _context.SubscribeToUser.Remove(subscribeToUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SubscribeToUserExists(int id)
        {
            return _context.SubscribeToUser.Any(e => e.ID == id);
        }
    }
}
