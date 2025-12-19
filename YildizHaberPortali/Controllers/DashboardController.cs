using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Repositories;

namespace YildizHaberPortali.Controllers
{
    [Authorize] 
    public class DashboardController : Controller
    {
        private readonly INewsRepository _newsRepository;

        public DashboardController(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<IActionResult> Index()
        {
            
            var allNews = await _newsRepository.GetAllWithCommentsAsync();
            ViewBag.TotalCount = allNews.Count;
            ViewBag.ActiveCount = allNews.Count(x => x.IsPublished);
            ViewBag.PassiveCount = allNews.Count(x => !x.IsPublished);

            var mostCommented = allNews
                .OrderByDescending(x => x.Comments.Count)
                .Take(5)
                .Select(x => new {
                    Title = x.Title.Length > 20 ? x.Title.Substring(0, 20) + "..." : x.Title,
                    CommentCount = x.Comments.Count
                })
                .ToList();

            ViewBag.ChartLabels = mostCommented.Select(x => x.Title).ToList();
            ViewBag.ChartData = mostCommented.Select(x => x.CommentCount).ToList();

            return View(allNews.OrderByDescending(x => x.CreatedDate).Take(5).ToList());
        }
    }
}