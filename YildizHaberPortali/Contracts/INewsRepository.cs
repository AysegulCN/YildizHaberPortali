using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface INewsRepository : IGenericRepository<News>
    {
        Task<IEnumerable<News>> GetByCategoryIdAsync(int categoryId);

        Task<List<News>> GetAllWithCategoryAsync();
        Task<List<News>> GetAllWithCommentsAsync();


    }
}