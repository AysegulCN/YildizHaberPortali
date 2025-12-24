using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

            var comments = await _commentRepository.GetAllWithNewsAsync();

            if (roles.Contains("Admin"))
                return View(comments.OrderByDescending(x => x.CreatedDate).ToList());

            var writerComments = comments.Where(c => c.News?.AuthorId == user.Id).OrderByDescending(x => x.CreatedDate).ToList();
            return View(writerComments);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostComment(int newsId, string name, string content)
        {
            if (string.IsNullOrEmpty(content))
                return Json(new { success = false, message = "Lütfen yorumunuzu yazın!" });

            var comment = new Comment
            {
                NewsId = newsId,
                UserName = name, 
                Text = content,
                CreatedDate = DateTime.Now,
                IsApproved = true 
            };

            await _commentRepository.AddAsync(comment);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", name, "Yeni bir yorum yaptı!");

            return Json(new { success = true, message = "Yorumunuz yayınlandı." });
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id) 
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return Json(new { success = false });

            comment.IsApproved = true;
            await _commentRepository.UpdateAsync(comment);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) 
        {
            await _commentRepository.DeleteAsync(id); 
            return Json(new { success = true });
        }
    }
}