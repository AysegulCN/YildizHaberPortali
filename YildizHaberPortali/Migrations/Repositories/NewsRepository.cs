// Repositories/NewsRepository.cs (Uyumlu ve Temiz Versiyon)
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YildizHaberPortali.Repositories
{
    public class NewsRepository : GenericRepository<News>, INewsRepository
    {

        public NewsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<News>> GetAllWithCategoryAsync()
        {
            // Veritabanından Haberleri + Kategorilerini (Include) alıp liste olarak dönüyoruz
            return await _context.News.Include(n => n.Category).ToListAsync();
        }
        public async Task<IEnumerable<News>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.News
                .Include(n => n.Category)
                .Where(n => n.CategoryId == categoryId)
                .ToListAsync();
        }


        public async Task<ICollection<News>> GetAllAsync()
        {
           
            var newsList = await _context.News
                .Include(n => n.Category) 
                .ToListAsync();

            return newsList
                .OrderBy(n => n.Priority)
                .ThenByDescending(n => n.PublishDate)
                .ToList();
        }

        public async Task<List<News>> GetAllWithCommentsAsync()
        {
            // Include(x => x.Comments) diyerek haberleri yorumlarıyla paketliyoruz
            return await _context.News
                .Include(x => x.Comments)
                .ToListAsync();
        }

    }
}