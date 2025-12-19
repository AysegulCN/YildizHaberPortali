

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using YildizHaberPortali.Helpers; // StringHelper için gerekli olan burası!

namespace YildizHaberPortali.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
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

        // --- AJAX İÇİN KATEGORİ EKLEME METODU ---
        [HttpPost]
        public async Task<IActionResult> CreateAjax(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new { success = false, message = "Kategori adı boş olamaz!" });

            var category = new Category
            {
                Name = name,
                Slug = StringHelper.ToSlug(name) // Helpers klasöründeki metodunu çağırır
            };

            await _categoryRepository.AddAsync(category);

            // JSON ile "Bitti, işte yeni veriler!" diyoruz
            return Json(new
            {
                success = true,
                id = category.Id,
                name = category.Name,
                slug = category.Slug
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return Json(new { success = false });

            await _categoryRepository.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}