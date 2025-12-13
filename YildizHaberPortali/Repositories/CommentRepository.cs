// Repositories/CommentRepository.cs

using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Repositories
{
    // CommentRepository, ICommentRepository'den miras almalı ve GenericRepository'nin Comment tipindeki versiyonundan miras almalı.
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        // Base class (GenericRepository) constructor'ını çağırıyoruz ve context'i atıyoruz.
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ICommentRepository'de özel metotlar tanımlanmadığı sürece (örneğin NewsId'ye göre yorum çekme), 
        // bu sınıf sadece GenericRepository'deki CRUD metotlarını kullanır.
    }
}