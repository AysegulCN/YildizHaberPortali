using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System; 
using System.Linq; 
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;



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

        // Controllers/NewsController.cs (Index metodu)

        public async Task<IActionResult> Index(int? categoryId)
        {
            IEnumerable<News> newsList;

            // Tüm kategorileri çek
            ViewBag.Categories = await _categoryRepository.GetAllAsync();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // LINQ sorgusu hatasızdır
                newsList = await _newsRepository.GetByCategoryIdAsync(categoryId.Value);

                // Kategori adını bulmanın güvenli yolu
                var category = ViewBag.Categories.FirstOrDefault(c => c.Id == categoryId.Value);
                ViewData["Title"] = $"{category?.Name} Haberleri";
            }
            else
            {
                newsList = await _newsRepository.GetAllAsync();
                ViewData["Title"] = "Tüm Haberler";
            }

            return View(newsList);
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
                    // Buradaki alanların News Model'inizdeki alanlarla eşleştiğinden emin olun
                    ImageUrl = viewModel.ImageUrl,
                    Author = viewModel.Author,
                    CategoryId = viewModel.CategoryId,
                    PublishDate = DateTime.Now,
                    // Eklediyseniz, IsPublished = true, // varsayılan
                };

                // HATA OLABİLECEK YER: _newsRepository.AddAsync(news) çağrısı
                await _newsRepository.AddAsync(news);

                // SignalR bildirimi (Daha önce eklemiştik)
                // await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Admin", $"Yeni Haber Eklendi: {news.Title}");

                return RedirectToAction(nameof(Index)); // Kayıt başarılıysa News/Index'e yönlendirir
            }
            // ... (Hata durumunda View'a geri dönme kodu)
            return View(viewModel);
        }

        // Controllers/NewsController.cs (Index metodu)

        public async Task<IActionResult> Index(int? categoryId)
        {
            IEnumerable<News> newsList;

            // Tüm kategorileri çek
            // Eğer View'a Category'i göndermek için farklı bir yol kullanmıyorsanız bu gereklidir.
            ViewBag.Categories = await _categoryRepository.GetAllAsync();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // Category ID'si varsa, o kategoriye ait haberleri getir
                newsList = await _newsRepository.GetByCategoryIdAsync(categoryId.Value);

                // CS1977 hatasını çözen, doğru LINQ sorgusu ile kategori adını bulma:
                var category = ViewBag.Categories.FirstOrDefault(c => c.Id == categoryId.Value);
                ViewData["Title"] = $"{category?.Name} Haberleri";
            }
            else
            {
                // ID yoksa tüm haberleri getir
                newsList = await _newsRepository.GetAllAsync();
                ViewData["Title"] = "Tüm Haberler";
            }

            return View(newsList);
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