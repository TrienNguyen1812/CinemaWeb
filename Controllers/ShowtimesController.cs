using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaWeb.Controllers
{
    public class ShowtimesController : Controller
    {
        private readonly DbContexts _context;

        public ShowtimesController(DbContexts context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var showtimes = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom);

            return View(await showtimes.ToListAsync());
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .FirstOrDefaultAsync(m => m.IdShowtime == id);

            if (showtime == null) return NotFound();

            return View(showtime);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewData["IdMovie"] = new SelectList(_context.Movies, "IdMovie", "MovieName");
            ViewData["IdRoom"] = new SelectList(_context.ScreeningRooms, "IdRoom", "RoomName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Showtime showtime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdMovie"] = new SelectList(_context.Movies, "IdMovie", "MovieName", showtime.IdMovie);
            ViewData["IdRoom"] = new SelectList(_context.ScreeningRooms, "IdRoom", "RoomName", showtime.IdRoom);
            return View(showtime);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            ViewData["IdMovie"] = new SelectList(_context.Movies, "IdMovie", "MovieName", showtime.IdMovie);
            ViewData["IdRoom"] = new SelectList(_context.ScreeningRooms, "IdRoom", "RoomName", showtime.IdRoom);
            return View(showtime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Showtime showtime)
        {
            if (id != showtime.IdShowtime) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(showtime);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.ScreeningRoom)
                .FirstOrDefaultAsync(m => m.IdShowtime == id);

            if (showtime == null) return NotFound();

            return View(showtime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
