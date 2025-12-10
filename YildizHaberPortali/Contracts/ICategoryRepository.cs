// Contracts/ICategoryRepository.cs

using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    // Artık tüm metotlar asenkron (Async) dönecek
    public interface ICategoryRepository
    {
        // Tüm kategorileri getirir (Asenkron)
        Task<ICollection<Category>> GetAllAsync();

        // Belirli bir ID'ye göre getirir (Asenkron)
        Task<Category> GetByIdAsync(int id);

        // Yeni bir kategori ekler (Asenkron)
        Task AddAsync(Category entity);

        // Kategoriyi günceller (Asenkron)
        Task UpdateAsync(Category entity);

        // Kategoriyi siler (Asenkron, ID üzerinden)
        Task DeleteAsync(int id);

        // Değişiklikleri kaydeder (Bu metot genellikle gereksizdi, ancak arkadaşınızın kodunu 
        // taklit etmek yerine daha modern bir yolu kullanacağız: Kayıt işlemi Add/Update/Delete içinde olacak)
    }
}