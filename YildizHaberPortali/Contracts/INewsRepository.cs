// Contracts/INewsRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface INewsRepository : IGenericRepository<News>
    {
        // Özel metot (Dönüş tipinin GenericRepository ile uyumlu olması için: IEnumerable<News>)
        Task<IEnumerable<News>> GetByCategoryIdAsync(int categoryId);

        Task<List<News>> GetAllWithCategoryAsync();
        Task<List<News>> GetAllWithCommentsAsync();


    }
}