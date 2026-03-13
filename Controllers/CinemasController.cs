using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaWeb.Controllers
{
    public class CinemasController : Controller
    {
        private readonly DbContexts _context;

        public CinemasController(DbContexts context)
        {
            _context = context;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas.ToListAsync();
            return View(cinemas);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            // 👉 Kiểm tra thiếu thuộc tính
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(c => c.IdCinema == id);

            if (cinema == null) return NotFound();

            return View(cinema);
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema)
        {
            if (id != cinema.IdCinema) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            _context.Update(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
