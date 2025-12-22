using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts 
{
    // IGenericRepository'den miras almazsan GetList() metodunu göremezsin!
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId);
    }
}