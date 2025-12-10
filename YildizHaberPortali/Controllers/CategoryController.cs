// Controllers/CategoryController.cs

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Authorization; // <<< YETKİLENDİRME İÇİN KRİTİK USING

namespace YildizHaberPortali.Controllers
{
    // Yetkilendirme Nitelikleri buraya gelmeli
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Slug oluşturma
            category.Slug = category.Name
                .ToLower()
                .Replace(" ", "-")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ç", "c");

            await _categoryRepository.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }

        // Edit ve Delete metotları da burada olmalıdır.
    }
}