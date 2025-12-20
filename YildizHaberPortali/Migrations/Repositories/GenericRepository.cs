using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// T: Herhangi bir sınıf tipi (Category, News, Comment vs.)
namespace YildizHaberPortali.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        // CS1729 hatasını çözmek için gerekli constructor:
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Generic Repository'nin CRUD metotları (CS0535 hatalarının bir kısmını çözmeye yardımcı olur)

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}