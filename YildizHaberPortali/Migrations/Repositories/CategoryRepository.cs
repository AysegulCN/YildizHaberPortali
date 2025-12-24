using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Data;





namespace YildizHaberPortali.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetAllAsync()
        {
            var categoryList = await _context.Categories.ToListAsync();

            return categoryList
                .OrderBy(c => c.DisplayOrder)
                .ToList();
        }


        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Category>> GetCategoriesWithLatestNewsAsync()
        {
            return await _context.Categories
                .Include(c => c.News)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    News = c.News.Where(n => n.IsPublished)
                                 .OrderByDescending(n => n.CreatedDate)
                                 .Take(4).ToList()
                }).ToListAsync();
        }

    }
}
