using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Contracts;

namespace YildizHaberPortali.ViewComponents
{
    public class CommentCountViewComponent : ViewComponent
    {
        private readonly ICommentRepository _commentRepository;

        public CommentCountViewComponent(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Veritabanından onaylanmamış yorum sayısını çekiyoruz
            var comments = await _commentRepository.GetAllAsync();
            int count = comments.Count(x => !x.IsApproved);
            return View(count); // Sayıyı View'a gönderiyoruz
        }
    }
}