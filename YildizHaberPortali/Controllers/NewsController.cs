using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using System.Linq; // Select, ToList kullanımı için
using System; // DateTime.Now kullanımı için


namespace YildizHaberPortali.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;

        public NewsController(INewsRepository newsRepository, ICategoryRepository categoryRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _newsRepository.GetAllAsync();
            return View(newsList);
        }


        public async Task<IActionResult> Create()
        {
            var categoryList = await _categoryRepository.GetAllAsync();

            var viewModel = new NewsCreateViewModel
            {
                Categories = categoryList.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    ImageUrl = viewModel.ImageUrl,
                    Author = viewModel.Author,
                    CategoryId = viewModel.CategoryId,
                    PublishDate = DateTime.Now,
                };

                await _newsRepository.AddAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var categoryList = await _categoryRepository.GetAllAsync();
            viewModel.Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ModelState.AddModelError("", "Haber eklenirken bir hata oluştu veya zorunlu alanlar eksik.");
            return View(viewModel);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var news = await _newsRepository.GetByIdAsync(id.GetValueOrDefault());

            if (news == null) return NotFound();

            var categoryList = await _categoryRepository.GetAllAsync();

            var viewModel = new NewsCreateViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                ImageUrl = news.ImageUrl,
                Author = news.Author,
                CategoryId = news.CategoryId,
                Categories = categoryList.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    Id = viewModel.Id,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    ImageUrl = viewModel.ImageUrl,
                    Author = viewModel.Author,
                    CategoryId = viewModel.CategoryId,
                    PublishDate = DateTime.Now,
                };

                await _newsRepository.UpdateAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var categoryList = await _categoryRepository.GetAllAsync();
            viewModel.Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ModelState.AddModelError("", "Haber güncellenirken bir hata oluştu.");
            return View(viewModel);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var news = await _newsRepository.GetByIdAsync(id.GetValueOrDefault());

            if (news == null) return NotFound();

            return View(news);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _newsRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}