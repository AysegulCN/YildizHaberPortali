using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        // 🚀 Admin paneli için haber başlıklarıyla beraber çekme
        Task<List<Comment>> GetAllWithNewsAsync();

        // 🚀 Haber detayı için onaylı yorumları çekme
        Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId);
    }
}