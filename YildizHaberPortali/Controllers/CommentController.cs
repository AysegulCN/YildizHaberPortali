using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Hubs;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Controllers
{
    [Authorize(Roles = "Admin,Yazar")]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository; 
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NewsHub> _hubContext; 

        public CommentController(ICommentRepository commentRepository,
                                 UserManager<AppUser> userManager,
                                 IHubContext<NewsHub> hubContext)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
            _hubContext = hubContext; 
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var roles = await _userManager.GetRolesAsync(user);
            var comments = await _commentRepository.GetAllAsync();

            if (roles.Contains("Admin")) return View(comments);

            return View(comments.Where(c => c.News?.AuthorId == user.Id).ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostComment(int newsId, string name, string content)
        {
            if (string.IsNullOrEmpty(content))
                return Json(new { success = false, message = "Lütfen yorumunuzu yazın!" });

            var user = await _userManager.GetUserAsync(User); // Giriş yapan kullanıcıyı al

            var comment = new Comment
            {
                NewsId = newsId,
                UserId = user?.Id, // Giriş yapmadıysa null olabilir veya anonim id atayabilirsin
                Text = content, // 🚀 Content yerine Text!
                CreatedDate = DateTime.Now, // 🚀 CommentDate yerine CreatedDate!
                IsApproved = true
            };

            await _commentRepository.AddAsync(comment);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", name, "Yeni bir yorum yapıldı!");

            return Json(new { success = true, message = "Yorumunuz yayınlandı." });
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment != null)
            {
                comment.IsApproved = true;
                await _commentRepository.UpdateAsync(comment);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentRepository.DeleteAsync(id);
            return Json(new { success = true });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int NewsId, string Text)
        {
            var user = await _userManager.GetUserAsync(User);

            var comment = new Comment
            {
                NewsId = NewsId,
                UserId = user.Id,
                Text = Text,
                IsApproved = true, // 🚀 Ayşegül'ün isteği: Anında yayınlanıyor!
                CreatedDate = DateTime.Now
            };

            await _commentRepository.AddAsync(comment);

            // 📢 Bildirim gelsin ama onay bekliyor demesin, sadece "Yeni yorum yazıldı" desin
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", user.FullName, "Haberinize yeni bir yorum bıraktı.");

            return RedirectToAction("Details", "News", new { id = NewsId });
        }

    }
}