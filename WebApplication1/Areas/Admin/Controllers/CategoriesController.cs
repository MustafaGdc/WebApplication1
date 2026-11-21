using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly IGenericRepository<Category> _categoryRepo;

        public CategoriesController(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _categoryRepo.GetAllAsync();
            return View(list.OrderByDescending(c => c.CreatedAt).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            await _categoryRepo.AddAsync(model);
            await _categoryRepo.SaveAsync();

            TempData["Success"] = "Klasör / kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = await _categoryRepo.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = model.Name;
            existing.Description = model.Description;

            _categoryRepo.Update(existing);
            await _categoryRepo.SaveAsync();

            TempData["Success"] = "Klasör / kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _categoryRepo.Delete(category);
            await _categoryRepo.SaveAsync();

            TempData["Success"] = "Klasör / kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}