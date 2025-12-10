// Repositories/NewsRepository.cs

using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace YildizHaberPortali.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<News>> GetAllAsync()
        {
            // Haberleri listelerken, hangi kategoriye ait olduğunu da çekmeliyiz (Include)
            return await _context.News
                                 .Include(n => n.Category) // Category Navigation Property'sini yükle
                                 .ToListAsync();
        }

        public async Task<News> GetByIdAsync(int id)
        {
            // ID'ye göre haberi ve ilişkili kategoriyi çek
            return await _context.News
                                 .Include(n => n.Category)
                                 .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(News entity)
        {
            await _context.News.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(News entity)
        {
            _context.News.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
        }
    }
}