using Microsoft.AspNetCore.Mvc;
using System; 
using System.Diagnostics;
using System.Linq; 
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using YildizHaberPortali.Repositories;

namespace YildizHaberPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;

        // 🚀 DÜZELTİLMİŞ CONSTRUCTOR (Virgül ve Parantez Hatası Giderildi)
        public HomeController(INewsRepository newsRepository,
                              ICategoryRepository categoryRepository,
                              ICommentRepository commentRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var allNews = await _newsRepository.GetAllWithCategoryAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var publishedNews = allNews.Where(x => x.IsPublished).OrderByDescending(x => x.CreatedDate).ToList();

            var model = new HomeViewModel();
            model.Categories = categories.ToList();
            model.SliderNews = publishedNews.Take(5).ToList(); 

            if (categoryId.HasValue)
            {
                model.LatestNews = publishedNews.Where(x => x.CategoryId == categoryId.Value).ToList();

                var selectedCat = categories.FirstOrDefault(c => c.Id == categoryId.Value);
                ViewBag.SelectedCategoryName = selectedCat?.Name;
            }
            else
            {
                model.LatestNews = publishedNews.Skip(5).Take(10).ToList();
            }

            model.MostReadNews = publishedNews.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Haberi tüm detaylarıyla çekiyoruz
            var news = await _newsRepository.GetByIdAsync(id);

            if (news == null)
            {
                return View("NotFound"); // Senin o "Tünelin Sonu Karanlık" sayfan
            }

            // 🚀 YENİ VİEWMODEL YAPILANDIRMASI
            var viewModel = new NewsDetailViewModel
            {
                News = news,
                Comments = new List<Comment>(), // Varsa yorumları buradan çekebilirsin
                NewComment = new Comment()
            };

            // Yan sütun için diğer haberleri ViewBag ile göndermeye devam edelim
            var allNews = await _newsRepository.GetAllAsync();
            ViewBag.RelatedNews = allNews.Where(x => x.IsPublished && x.Id != id).Take(5).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(int NewsId, string UserName, string Text)
        {
            var comment = new Comment
            {
                NewsId = NewsId,
                UserName = UserName,
                Text = Text,
                CreatedDate = DateTime.Now,
                IsApproved = false 
            };

            await _commentRepository.AddAsync(comment); 

            TempData["SuccessMessage"] = "Yorumunuz alındı, admin onayından sonra yayınlanacaktır!";
            return RedirectToAction("Details", new { id = NewsId });
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Bize Ulaşın";
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ErrorPage(int? code)
        {
            if (code == 404)
            {
                return View("NotFound"); 
            }

            return View("Error");
        }
    }
}