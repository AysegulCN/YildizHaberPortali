// Repositories/NewsRepository.cs (Hatasız ve Tam Versiyon)

using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YildizHaberPortali.Repositories
{
    // GenericRepository<News> miras alınır, INewsRepository uygulanır.
    public class NewsRepository : GenericRepository<News>, INewsRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor: GenericRepository'ye DbContext'i göndeririz.
        public NewsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // INewsRepository'den gelen özel metot implementasyonu
        public async Task<IEnumerable<News>> GetByCategoryIdAsync(int categoryId)
        {
            // Kategori bilgisini dahil et (Include)
            return await _context.News
                .Include(n => n.Category)
                .Where(n => n.CategoryId == categoryId)
                .ToListAsync();
        }

        // GenericRepository'deki GetAllAsync metodunu override etmek veya gizlemek isteyebilirsiniz.
        // Örneğin, kategori dahil çekmek için:
        public new async Task<IEnumerable<News>> GetAllAsync()
        {
            return await _context.News
                .Include(n => n.Category)
                .ToListAsync();
        }
    }
}