using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using YildizHaberPortali.Contracts;

namespace YildizHaberPortali.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Comment>> GetAllWithNewsAsync()
        {
            return await _context.Comments
                .Include(x => x.News) 
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId)
        {
            return await _context.Comments
                .Where(x => x.NewsId == newsId && x.IsApproved)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}