using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Authorization; // [Authorize] niteliği için bu using gereklidir

namespace YildizHaberPortali.Controllers
{
    // [Authorize] niteliği, sınıf tanımının hemen üstünde yer almalıdır.
    // Bu, sadece "Admin" rolüne sahip kullanıcıların bu Controller'a erişebileceği anlamına gelir.
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // ---------------------------------------------------------------------
        // R - Read (Oku) İşlemi: Index
        // ---------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // ---------------------------------------------------------------------
        // C - Create (Oluştur) İşlemleri
        // ---------------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            // Model Validasyonu kontrol edilir (Örn: Name alanı boş mu?)
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Slug oluşturma (Türkçe karakterleri temizleyerek)
            category.Slug = category.Name
                .ToLower()
                .Replace(" ", "-")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ç", "c");

            // Kayıt işlemi (ICategoryRepository'nin AddAsync metodu çağrılır)
            await _categoryRepository.AddAsync(category);

            // Başarılı olursa Index sayfasına yönlendir
            return RedirectToAction(nameof(Index));
        }

        // DİKKAT: Edit ve Delete metotları da bu sınıfın içine eklenmelidir (Daha önce size vermiştim).
        // Şu an sadece Create metotlarını koruduk.
    }
}