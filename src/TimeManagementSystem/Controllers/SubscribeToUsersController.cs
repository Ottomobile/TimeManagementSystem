using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeManagementSystem.Data;
using TimeManagementSystem.Models;
using System.Data;

namespace TimeManagementSystem.Controllers
{
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
            return View(await _context.SubscribeToUser.ToListAsync());
        }

        // GET: SubscribeToUsers/Add
        public IActionResult Add()
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
                subscribeToUser.CurrentUser = this.User.Identity.Name;

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
                    ModelState.AddModelError(string.Empty, "User entered is not a Manager.");
                }
            }
            return View(subscribeToUser);
        }

        // GET: SubscribeToUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscribeToUser = await _context.SubscribeToUser.SingleOrDefaultAsync(m => m.ID == id);
            if (subscribeToUser == null)
            {
                return NotFound();
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
