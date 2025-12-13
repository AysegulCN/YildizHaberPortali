// Controllers/CommentController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using System.Linq;
using System;

// Sadece Admin ve Yazar (Editor) erişebilir
[Authorize(Roles = "Admin, Editor")]
public class CommentController : Controller
{
    private readonly ICommentRepository _commentRepository;
    // Eğer SignalR kullandıysak: private readonly IHubContext<NewsHub> _hubContext;

    public CommentController(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    // GET: /Comment/Index (Admin Panelinde Tüm Yorumları Listeleme)
    public async Task<IActionResult> Index()
    {
        var comments = await _commentRepository.GetAllAsync();
        // Yorumları en yeniden eskiye sıralayalım
        return View(comments.OrderByDescending(c => c.CommentDate));
    }

    // POST: /Comment/Delete (AJAX ile Yorum Silme)
    [HttpPost]
    [Authorize(Roles = "Admin")] // Sadece Admin silebilir
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            return Json(new { success = false, message = "Yorum bulunamadı." });
        }

        await _commentRepository.DeleteAsync(id);
        return Json(new { success = true, message = "Yorum başarıyla silindi." });
    }

    // --------------- Halka Açık Yorum Ekleme Metodu ----------------

    [HttpPost]
    [AllowAnonymous] // Herkes yorum ekleyebilir
    public async Task<IActionResult> AddComment(Comment comment)
    {
        if (ModelState.IsValid)
        {
            comment.CommentDate = DateTime.Now;
            await _commentRepository.AddAsync(comment);

            // Kullanıcıyı, yorum yaptığı haberin detay sayfasına geri yönlendir.
            return RedirectToAction("Details", "Home", new { id = comment.NewsId });
        }

        // Model geçerli değilse, detay sayfasına geri dön.
        return RedirectToAction("Details", "Home", new { id = comment.NewsId });
    }
}