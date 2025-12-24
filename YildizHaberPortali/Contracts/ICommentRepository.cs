using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetAllWithNewsAsync();

        Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId);
    }
}