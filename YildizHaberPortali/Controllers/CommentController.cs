using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Hubs;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHubContext<NewsHub> _hubContext;

        public CommentController(ICommentRepository commentRepository, IHubContext<NewsHub> hubContext)
        {
            _commentRepository = commentRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var comments = await _commentRepository.GetAllAsync();
            return View(comments.OrderByDescending(x => x.CommentDate).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(int newsId, string name, string content)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content))
            {
                return Json(new { success = false, message = "Lütfen adınızı ve yorumunuzu yazın!" });
            }

            var comment = new Comment
            {
                NewsId = newsId,
                AuthorName = name,      
                Content = content,
                CommentDate = DateTime.Now, 
                IsApproved = false      
            };

            await _commentRepository.AddAsync(comment);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", name, "Yeni bir yorum onayınızı bekliyor!");

            return Json(new { success = true, message = "Yorumunuz alındı, onaylandıktan sonra görünecektir." });
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
        [AllowAnonymous]
        public async Task<IActionResult> AddComment(int newsId, string authorName, string content)
        {
            if (string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(content))
            {
                TempData["CommentError"] = "Lütfen tüm alanları doldurun.";

                return RedirectToAction("Details", "News", new { id = newsId });
            }

            var comment = new Comment
            {
                NewsId = newsId,
                AuthorName = authorName,
                Content = content,
                CommentDate = DateTime.Now,
                IsApproved = false 
            };

            await _commentRepository.AddAsync(comment);

            TempData["CommentSuccess"] = "Yorumunuz alındı, onaylandıktan sonra görünecektir.";
            return RedirectToAction("Details", "News", new { id = newsId });
        }
    }
}