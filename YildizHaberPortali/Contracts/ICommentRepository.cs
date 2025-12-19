using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<List<Comment>> GetApprovedCommentsByNewsIdAsync(int newsId);
}