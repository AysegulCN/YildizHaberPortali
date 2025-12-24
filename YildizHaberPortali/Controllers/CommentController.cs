using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Hubs;
using YildizHaberPortali.Models;
using System;

namespace YildizHaberPortali.Controllers
{
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

        [Authorize(Roles = "Admin,Yazar")]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment(int newsId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Lütfen yorumunuzu yazın!";
                return RedirectToAction("Details", "News", new { id = newsId });
            }

            string commenterName = User.Identity.IsAuthenticated ? User.Identity.Name : "Misafir Okuyucu";

            var comment = new Comment
            {
                NewsId = newsId,
                UserName = commenterName,
                Text = content,
                CreatedDate = DateTime.Now,
                IsApproved = false
            };

            await _commentRepository.AddAsync(comment);

            
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", commenterName, content);
            

            TempData["SuccessMessage"] = "Yorumunuz alındı, yönetici onayından sonra yayınlanacaktır.";

            return RedirectToAction("Details", "News", new { id = newsId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return Json(new { success = false });

            comment.IsApproved = true;
            await _commentRepository.UpdateAsync(comment);

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentRepository.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}