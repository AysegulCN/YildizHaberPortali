
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Authorization; 

namespace YildizHaberPortali.Controllers
{
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
                
                category.Slug = GenerateSlug(category.Name);

                
                await _categoryRepository.AddAsync(category);

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();

            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-").Trim();

            str = System.Text.RegularExpressions.Regex.Replace(str, @"-+", "-");

            return str.Length > 45 ? str.Substring(0, 45) : str;
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryRepository.DeleteAsync(id);

                return Json(new { success = true, message = "Kategori başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
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