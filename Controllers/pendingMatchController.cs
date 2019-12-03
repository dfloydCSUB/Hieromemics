using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hieromemics.Data;
using Hieromemics.Models;

namespace Hieromemics.Controllers
{
    public class pendingMatchController : Controller
    {
        private readonly HieromemicsContext _context;

        public pendingMatchController(HieromemicsContext context)
        {
            _context = context;
        }

        // GET: pendingMatch
        public async Task<IActionResult> Index(int uid, string fguid)
        {
            var existing = from e in _context.pendingMatch
                         select e;
            var friend = from f in _context.users
                         select f;
            var pending = from p in _context.pendingMatch
                            select p;

            if (!String.IsNullOrEmpty(fguid))
            {
                friend = friend.Where(f => f.FriendCode == fguid);

                //existing = existing.Where(e => e.lookingId == friend.Single().UserID && e.seekingId== uid);

                if(uid != friend.Single().UserID)
                {
                    
                }
                else if (existing.Count() == 0)
                {
                    pendingMatch pend = new pendingMatch();
                    pend.lookingId = uid;
                    //pend.seekingId = friend.Single().UserID;
                    await Create(pend);
                }
                else
                {
                    friendListController flc = new friendListController(_context);
                    friendList fren1 = new friendList();
                    friendList fren2 = new friendList();
                    fren1.UserID = uid;
                    fren1.FriendCode = fguid;
                    fren2.UserID = friend.Single().UserID;
                    var usr = from u in _context.users 
                                select u;
                    fren2.FriendCode = usr
                                        .Where(u => u.UserID == uid)
                                        .Select(u => u.FriendCode)
                                        .Single();
                    await flc.Create(fren1);
                    await flc.Create(fren2);
                }
            }

            return View(await pending.Where(p => p.lookingId == uid).ToListAsync());
        }

        // GET: pendingMatch/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendingMatch = await _context.pendingMatch
                .FirstOrDefaultAsync(m => m.pendingId == id);
            if (pendingMatch == null)
            {
                return NotFound();
            }

            return View(pendingMatch);
        }

        // GET: pendingMatch/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: pendingMatch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("pendingId,lookingId,seekingId")] pendingMatch pendingMatch)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(pendingMatch);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                var existing = from e in _context.pendingMatch
                         select e;

                var friend = from f in _context.users
                         select f;
                
                var pending = from p in _context.pendingMatch
                            select p;

                var temp = friend.Where(f => f.UserID == pendingMatch.lookingId);

                friend = friend.Where(f => f.FriendCode == pendingMatch.seekingId);

                existing = existing.Where(e => e.lookingId == friend.Single().UserID && e.seekingId== temp.Single().FriendCode);

                if (pendingMatch.lookingId != friend.Single().UserID)
                {

                }
                else if (existing.Count() == 0)
                {
                    await Create(pendingMatch);
                }
                else
                {
                    friendListController flc = new friendListController(_context);
                    friendList fren1 = new friendList();
                    friendList fren2 = new friendList();
                    var usr = from u in _context.users 
                                select u;
                    fren1.UserID = pendingMatch.lookingId;
                    fren1.FriendCode = usr
                                        .Where(u => u.FriendCode == pendingMatch.seekingId)
                                        .Select(u => u.FriendCode)
                                        .Single();
                    fren2.UserID = friend.Single().UserID;
                    fren2.FriendCode = usr
                                        .Where(u => u.UserID == pendingMatch.lookingId)
                                        .Select(u => u.FriendCode)
                                        .Single();
                    await flc.Create(fren1);
                    await flc.Create(fren2);
                }
                return View(await pending.Where(p => p.lookingId == pendingMatch.lookingId).ToListAsync());
            }
            return View(pendingMatch);
        }

        // GET: pendingMatch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendingMatch = await _context.pendingMatch.FindAsync(id);
            if (pendingMatch == null)
            {
                return NotFound();
            }
            return View(pendingMatch);
        }

        // POST: pendingMatch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("pendingId,lookingId,seekingId")] pendingMatch pendingMatch)
        {
            if (id != pendingMatch.pendingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendingMatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!pendingMatchExists(pendingMatch.pendingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pendingMatch);
        }

        // GET: pendingMatch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendingMatch = await _context.pendingMatch
                .FirstOrDefaultAsync(m => m.pendingId == id);
            if (pendingMatch == null)
            {
                return NotFound();
            }

            return View(pendingMatch);
        }

        // POST: pendingMatch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pendingMatch = await _context.pendingMatch.FindAsync(id);
            _context.pendingMatch.Remove(pendingMatch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool pendingMatchExists(int id)
        {
            return _context.pendingMatch.Any(e => e.pendingId == id);
        }
    }
}
