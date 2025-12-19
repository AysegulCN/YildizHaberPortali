using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using System; 
using System.Linq; 

namespace YildizHaberPortali.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(INewsRepository newsRepository, ICategoryRepository categoryRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var allNews = await _newsRepository.GetAllWithCategoryAsync();
            var publishedNews = allNews.Where(x => x.IsPublished).OrderByDescending(x => x.CreatedDate).ToList();

            var model = new HomeViewModel();
            model.SliderNews = publishedNews.Take(5).ToList();
            model.LatestNews = publishedNews.Skip(5).Take(4).ToList();

            var categories = await _categoryRepository.GetCategoriesWithLatestNewsAsync();
            model.Categories = categories.Where(c => c.News.Any()).ToList();

            model.MostReadNews = publishedNews.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            return View(model);
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Bize Ulaþýn";
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
                return View("NotFound"); // 404 ise özel tasarýmýmýza git
            }

            // Diðer hatalar için (500 vs.) standart hata sayfasýna git
            return View("Error");
        }
    }
}