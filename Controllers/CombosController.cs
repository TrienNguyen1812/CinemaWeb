using CinemaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWeb.Controllers
{
    public class CombosController : Controller
    {
        private readonly DbContexts _context;

        public CombosController(DbContexts context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var combos = _context.Combos.ToList();
            return View(combos);
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

            return RedirectToAction("Index");
        }
    }
}
