// Contracts/INewsRepository.cs

using YildizHaberPortali.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YildizHaberPortali.Contracts
{
    public interface INewsRepository
    {
        // Tüm haberleri ve ilişkili kategorilerini getir (Include ile Category'i de çekmek zorundayız)
        Task<ICollection<News>> GetAllAsync();

        // Belirli bir haberi getir
        Task<News> GetByIdAsync(int id);

        // Haber ekle
        Task AddAsync(News entity);

        // Haber güncelle
        Task UpdateAsync(News entity);

        // Haber sil
        Task DeleteAsync(int id);
    }
}