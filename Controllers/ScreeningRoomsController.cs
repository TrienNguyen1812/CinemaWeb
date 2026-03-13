using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaWeb.Controllers
{
    public class ScreeningRoomsController : Controller
    {
        private readonly DbContexts _context;

        public ScreeningRoomsController(DbContexts context)
        {
            _context = context;
        }

        // ===== INDEX =====
        public async Task<IActionResult> Index()
        {
            var rooms = _context.ScreeningRooms
                                .Include(r => r.Cinema);
            return View(await rooms.ToListAsync());
        }

        // ===== DETAILS =====
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.ScreeningRooms
                .Include(r => r.Cinema)
                .FirstOrDefaultAsync(m => m.IdRoom == id);

            if (room == null) return NotFound();

            return View(room);
        }

        // ===== CREATE =====
        public IActionResult Create()
        {
            ViewData["IdCinema"] = new SelectList(_context.Cinemas, "IdCinema", "CinemaName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScreeningRoom room)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdCinema"] = new SelectList(_context.Cinemas, "IdCinema", "CinemaName", room.IdCinema);
                return View(room);
            }

            _context.Add(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===== EDIT =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.ScreeningRooms.FindAsync(id);
            if (room == null) return NotFound();

            ViewData["IdCinema"] = new SelectList(_context.Cinemas, "IdCinema", "CinemaName", room.IdCinema);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScreeningRoom room)
        {
            if (id != room.IdRoom) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdCinema"] = new SelectList(_context.Cinemas, "IdCinema", "CinemaName", room.IdCinema);
                return View(room);
            }

            _context.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===== DELETE =====
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.ScreeningRooms
                .Include(r => r.Cinema)
                .FirstOrDefaultAsync(m => m.IdRoom == id);

            if (room == null) return NotFound();

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.ScreeningRooms.FindAsync(id);
            _context.ScreeningRooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
