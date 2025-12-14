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
        private readonly ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
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
        
    }
}