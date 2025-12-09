using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Models; 
namespace YildizHaberPortali.Controllers
{
    public class CategoryController : Controller
    {
        // CRUD işlemlerinde kullanılacak olan DbContext veya Repository buraya eklenecek
        // private readonly ApplicationDbContext _context;

        // Geçici olarak sizin istediğiniz kategorileri listelemek için kullanacağız
        private List<Category> GetSampleCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Dünya", Slug = "dunya" },
                new Category { Id = 2, Name = "Spor", Slug = "spor" },
                new Category { Id = 3, Name = "Ekonomi", Slug = "ekonomi" },
                new Category { Id = 4, Name = "Kadın", Slug = "kadin" },
                new Category { Id = 5, Name = "Teknoloji", Slug = "teknoloji" }
            };
        }

        public IActionResult Index()
        {
            // İleride burası _repository.GetAllCategories() olacak
            var categories = GetSampleCategories();
            return View(categories);
        }

        // Buraya Create, Edit, Delete metotları eklenecek
    }
}