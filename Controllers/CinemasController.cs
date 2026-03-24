using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaWeb.Services.Notifications;

namespace CinemaWeb.Controllers
{
    public class CinemasController : Controller
    {
        private readonly DbContexts _context;
        private readonly INotificationSubject _notificationSubject;

        public CinemasController(DbContexts context, INotificationSubject notificationSubject)
        {
            _context = context;
            _notificationSubject = notificationSubject;
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
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Cinemas.Add(cinema);
                    await _context.SaveChangesAsync();
                    _notificationSubject.Publish("Thêm rạp chiếu thành công!", "success");
                    return RedirectToAction(nameof(Index));
                }
                _notificationSubject.Publish("Dữ liệu nhập vào chưa đúng, vui lòng kiểm tra lại!", "warning");
                return View(cinema);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                _notificationSubject.Publish("Không thể thêm rạp chiếu. Vui lòng thử lại!", "error");
    
                ModelState.AddModelError("", "Lỗi chi tiết: " + ex.Message);
            }
            return View(cinema);
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
            _notificationSubject.Publish("Cập nhật rạp chiếu thành công!", "info");
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
                _notificationSubject.Publish("Xóa rạp chiếu thành công!", "warning");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
