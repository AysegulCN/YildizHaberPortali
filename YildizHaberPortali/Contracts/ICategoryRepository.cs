// Contracts/ICategoryRepository.cs

using YildizHaberPortali.Models;

namespace YildizHaberPortali.Contracts
{
    public interface ICategoryRepository
    {
        // Tüm kategorileri getir
        ICollection<Category> GetAll();

        // Belirli bir ID'ye göre kategoriyi getir
        Category GetById(int id);

        // Yeni bir kategori ekle
        bool Add(Category entity);

        // Kategoriyi güncelle
        bool Update(Category entity);

        // Kategoriyi sil
        bool Delete(Category entity);

        // Değişiklikleri kaydet (Genellikle Generic Repository'de bulunur, burada da kullanalım)
        bool Save();
    }
}