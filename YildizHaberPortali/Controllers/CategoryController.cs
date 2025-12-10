using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using System.Threading.Tasks;

namespace YildizHaberPortali.Controllers
{
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
    }
}
