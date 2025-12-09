using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection (Bağımlılık Enjeksiyonu) ile DbContext'i alıyoruz
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Category> GetAll()
        {
            // Tüm kategorileri veri tabanından getir
            return _context.Categories.ToList();
        }

        public Category GetById(int id)
        {
            // Belirli bir kategoriyi ID'ye göre getir
            return _context.Categories.FirstOrDefault(q => q.Id == id);
        }

        public bool Add(Category entity)
        {
            _context.Categories.Add(entity);
            return Save(); // Değişiklikleri kaydetmeyi deniyoruz
        }

        public bool Update(Category entity)
        {
            _context.Categories.Update(entity);
            return Save();
        }

        public bool Delete(Category entity)
        {
            _context.Categories.Remove(entity);
            return Save();
        }

        // Kaydetme işlemi
        public bool Save()
        {
            // Kaydedilen kayıt sayısının 0'dan büyük olup olmadığını kontrol et
            var changes = _context.SaveChanges();
            return changes > 0;
        }
    }
}