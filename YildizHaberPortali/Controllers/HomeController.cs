// Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Contracts;
using System.Threading.Tasks;
using YildizHaberPortali.Models; 
using System.Diagnostics; 

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
            var newsList = await _newsRepository.GetAllAsync();
            return View(newsList);
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
    }
}