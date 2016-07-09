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

        // GET: SubscribeToUsers/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: SubscribeToUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubscribeToUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CurrentUser,ManagingUser")] SubscribeToUser subscribeToUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscribeToUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subscribeToUser);
        }

        // GET: SubscribeToUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: SubscribeToUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CurrentUser,ManagingUser")] SubscribeToUser subscribeToUser)
        {
            if (id != subscribeToUser.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscribeToUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscribeToUserExists(subscribeToUser.ID))
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
