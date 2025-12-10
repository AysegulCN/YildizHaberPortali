// Controllers/NewsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using YildizHaberPortali.Repositories;

namespace YildizHaberPortali.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;

        // DI ile News Repository'yi alıyoruz
        public NewsController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        // GET: /News
        public async Task<IActionResult> Index()
        {
            var newsList = await _newsRepository.GetAllAsync();
            return View(newsList);
        }

        // Buraya Create, Edit, Delete Action'ları eklenecek
    }

    // GET: /News/Create
        public async Task<IActionResult> Create()
        {
            // Tüm kategorileri çek
            var categoryList = await _categoryRepository.GetAllAsync();

            // ViewModel'i oluştur ve Categories alanını doldur
            var viewModel = new NewsCreateViewModel
            {
                // Kategorileri SelectListItem'a dönüştürerek ViewModel'e ekliyoruz
                Categories = categoryList.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // ViewModel'den Model'e Dönüşüm (Mapping)
                var news = new News
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    ImageUrl = viewModel.ImageUrl,
                    Author = viewModel.Author,
                    CategoryId = viewModel.CategoryId,
                    PublishDate = DateTime.Now, // Yayın tarihini otomatik ata
                };

                await _newsRepository.AddAsync(news);
                return RedirectToAction(nameof(Index));
            }

            // Model geçerli değilse, kategorileri tekrar çekip View'ı döndürmeliyiz
            var categoryList = await _categoryRepository.GetAllAsync();
            viewModel.Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ModelState.AddModelError("", "Haber eklenirken bir hata oluştu veya zorunlu alanlar eksik.");
            return View(viewModel);
        }
    }
}
}