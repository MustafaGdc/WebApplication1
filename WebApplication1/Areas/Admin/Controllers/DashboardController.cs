using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IFileRepository _fileRepo;

        public DashboardController(
            IGenericRepository<Category> categoryRepo,
            IFileRepository fileRepo)
        {
            _categoryRepo = categoryRepo;
            _fileRepo = fileRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var files = await _fileRepo.GetAllWithCategoryAsync();

            ViewBag.CategoryCount = categories.Count();
            ViewBag.FileCount = files.Count();
            ViewBag.TotalDownloads = files.Sum(f => f.DownloadCount);
            ViewBag.TotalSizeKb = Math.Round(files.Sum(f => f.FileSize) / 1024.0, 2);

            return View(files.Take(5).ToList());
        }
    }
}