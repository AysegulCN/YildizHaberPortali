// Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics; 
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models; 

namespace YildizHaberPortali.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly INewsRepository _newsRepository;

        public HomeController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<News> newsList;

            if (User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                // Yetkililer her þeyi görür
                newsList = await _newsRepository.GetAllAsync();
            }
            else
            {
                // Normal kullanýcý sadece yayýnlananlarý görür
                newsList = (await _newsRepository.GetAllAsync())
                            .Where(n => n.IsPublished);
            }

            return View(newsList);
        }

        // Controllers/HomeController.cs içinde

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