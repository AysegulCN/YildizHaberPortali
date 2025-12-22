using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using YildizHaberPortali.Contracts; // 🚀 Arayüzü (Interface) buradan çağırıyoruz

namespace YildizHaberPortali.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId)
        {
            return await _context.Comments
                .Where(x => x.NewsId == newsId && x.IsApproved)
                .OrderByDescending(x => x.CreatedDate) // 🚀 SqlException'ı bu isimle çözeceğiz
                .ToListAsync();
        }
    }
}