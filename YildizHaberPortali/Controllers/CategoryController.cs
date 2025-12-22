using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Helpers;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsRepository _newsRepository; // EKLENDİ

        public CategoryController(ICategoryRepository categoryRepository, INewsRepository newsRepository)
        {
            _categoryRepository = categoryRepository;
            _newsRepository = newsRepository; // ENJEKTE EDİLDİ
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new { success = false, message = "Kategori adı boş olamaz!" });

            var category = new Category
            {
                Name = name,
                Slug = StringHelper.ToSlug(name)
            };

            await _categoryRepository.AddAsync(category);
            return Json(new { success = true, id = category.Id, name = category.Name, slug = category.Slug });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            var newsInCategory = await _newsRepository.GetAllAsync();
            if (newsInCategory.Any(x => x.CategoryId == id))
            {
                TempData["Error"] = "Bu kategoriye bağlı haberler olduğu için silemezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}