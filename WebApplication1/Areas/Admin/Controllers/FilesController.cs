using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FilesController : Controller
    {
        private readonly IFileRepository _fileRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IWebHostEnvironment _environment;

        public FilesController(
            IFileRepository fileRepo,
            IGenericRepository<Category> categoryRepo,
            IWebHostEnvironment environment)
        {
            _fileRepo = fileRepo;
            _categoryRepo = categoryRepo;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var files = await _fileRepo.GetAllWithCategoryAsync();
            return View(files);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new FileCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FileCreateViewModel model)
        {
            if (model.UploadFile == null || model.UploadFile.Length == 0)
            {
                ModelState.AddModelError(nameof(model.UploadFile), "Lütfen geçerli bir dosya seçin.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(model.UploadFile!.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(uploadsFolder, storedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.UploadFile.CopyToAsync(stream);
            }

            var entity = new StoredFile
            {
                FileName = model.FileName.Trim(),
                StoredFileName = storedFileName,
                FileExtension = extension.ToLowerInvariant(),
                FileSize = model.UploadFile.Length,
                FilePath = $"/uploads/{storedFileName}",
                DownloadCount = 0,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.Now
            };

            await _fileRepo.AddAsync(entity);
            await _fileRepo.SaveAsync();

            TempData["Success"] = "Dosya başarıyla yüklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            await LoadCategoriesAsync();

            var vm = new FileEditViewModel
            {
                Id = file.Id,
                FileName = file.FileName,
                CategoryId = file.CategoryId,
                IsActive = file.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            var file = await _fileRepo.GetByIdAsync(model.Id);
            if (file == null)
            {
                return NotFound();
            }

            file.FileName = model.FileName.Trim();
            file.CategoryId = model.CategoryId;
            file.IsActive = model.IsActive;

            _fileRepo.Update(file);
            await _fileRepo.SaveAsync();

            TempData["Success"] = "Dosya bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            string fullPath = Path.Combine(_environment.WebRootPath, "uploads", file.StoredFileName);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("Dosya sunucuda bulunamadı.");
            }

            file.DownloadCount += 1;
            _fileRepo.Update(file);
            await _fileRepo.SaveAsync();

            string downloadName = string.IsNullOrWhiteSpace(Path.GetExtension(file.FileName))
                ? $"{file.FileName}{file.FileExtension}"
                : file.FileName;

            return PhysicalFile(fullPath, "application/octet-stream", downloadName);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActiveAjax(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if (file == null)
            {
                return Json(new { success = false, message = "Dosya bulunamadı." });
            }

            file.IsActive = !file.IsActive;
            _fileRepo.Update(file);
            await _fileRepo.SaveAsync();

            return Json(new
            {
                success = true,
                isActive = file.IsActive,
                statusText = file.IsActive ? "Aktif" : "Pasif"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if (file == null)
            {
                return Json(new { success = false, message = "Dosya bulunamadı." });
            }

            string fullPath = Path.Combine(_environment.WebRootPath, "uploads", file.StoredFileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _fileRepo.Delete(file);
            await _fileRepo.SaveAsync();

            return Json(new { success = true, message = "Dosya silindi." });
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(
                categories.OrderBy(c => c.Name),
                "Id",
                "Name");
        }
    }
}