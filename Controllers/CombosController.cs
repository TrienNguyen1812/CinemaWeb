using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaWeb.Services.Notifications;

namespace CinemaWeb.Controllers
{
    public class CombosController : Controller
    {
        private readonly DbContexts _context;
        private readonly INotificationSubject _notificationSubject;

        public CombosController(DbContexts context, INotificationSubject notificationSubject)
        {
            _context = context;
            _notificationSubject = notificationSubject;
        }

        public IActionResult Index()
        {
            var combos = _context.Combos.ToList();
            return View(combos);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null) return NotFound();
            return View(combo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Combo combo, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);

                string path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                combo.Image = fileName;
            }

            _context.Combos.Add(combo);
            _context.SaveChanges();
            _notificationSubject.Publish("Thêm combo thành công!", "success");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var combo = _context.Combos.Find(id);

            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost]
        public IActionResult Edit(Combo combo, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);

                string path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                combo.Image = fileName;
            }

            _context.Combos.Update(combo);
            _context.SaveChanges();
            _notificationSubject.Publish("Cập nhật combo thành công!", "info");
            return RedirectToAction("Index");
        }

       [HttpGet]
        public IActionResult Delete(int id)
        {
            var combo = _context.Combos.Find(id);

            if (combo == null)
                return NotFound();

            return View(combo);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var combo = _context.Combos.Find(id);

            if (combo == null)
                return NotFound();

            _context.Combos.Remove(combo);
            _context.SaveChanges();
            _notificationSubject.Publish("Xóa combo thành công!", "warning");
            return RedirectToAction("Index");
        }
    }
}
