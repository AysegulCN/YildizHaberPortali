// Contracts/ICommentRepository.cs

using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    // Generic Repository'den miras alıyoruz
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        // Yorumlarla ilgili özel metotlar buraya eklenebilir.
        // Örneğin: Task<IEnumerable<Comment>> GetCommentsByNewsIdAsync(int newsId);
    }
}