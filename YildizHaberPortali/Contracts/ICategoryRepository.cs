using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();

        Task<Category> GetByIdAsync(int id);

        Task AddAsync(Category entity);

        Task UpdateAsync(Category entity);

        Task DeleteAsync(int id);

        Task<List<Category>> GetCategoriesWithLatestNewsAsync();

        
    }
}