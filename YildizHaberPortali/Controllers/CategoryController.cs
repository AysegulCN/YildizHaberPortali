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
            if (ModelState.IsValid)
            {
                // 1. Kategori adına göre URL dostu slug oluşturma
                // System.Text.RegularExpressions ve System.Globalization usingleri gerektirir
                category.Slug = GenerateSlug(category.Name);

                // 2. Kategori ekleme işlemi
                await _categoryRepository.AddAsync(category);

                // 3. Admin panelindeki Kategori Listesi sayfasına yönlendir.
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();

            // Geçersiz karakterleri temizle
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // Boşlukları tire ile değiştir
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-").Trim();

            // Ardışık tireleri tek tire ile değiştir
            str = System.Text.RegularExpressions.Regex.Replace(str, @"-+", "-");

            return str.Length > 45 ? str.Substring(0, 45) : str;
        }



        // Edit ve Delete metotları da burada olmalıdır.
    }
}