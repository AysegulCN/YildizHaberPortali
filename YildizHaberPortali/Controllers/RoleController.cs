// Controllers/RoleController.cs

using Microsoft.AspNetCore.Authorization; // Yetkilendirme için
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

[Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanıcılar erişebilir (İleride kullanılacak)
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    // GET: /Role/Index (Rol Listesi)
    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    // GET: /Role/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            // ... (Rol oluşturma kodların burada) ...
            var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

            if (result.Succeeded)
            {
                // HATA BURADAYDI: Muhtemelen burada return View("Index") veya return View("Başarılı") yazıyordu.
                // DOĞRUSU BU: İş bitince Listeye (Index sayfasına) yönlendir.
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        // Hata varsa sayfayı (ve modeli) tekrar göster ki kullanıcı düzeltsin
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        // 1. Rolü bul
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            return Json(new { success = false, message = "Rol bulunamadı!" });
        }

        // 🛡️ GÜVENLİK ÖNLEMİ: Admin rolü silinemez!
        if (role.Name == "Admin")
        {
            return Json(new { success = false, message = "Ana Yönetici (Admin) rolü silinemez!" });
        }

        // 2. Rolü sil
        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Rol başarıyla silindi." });
        }

        return Json(new { success = false, message = "Silme işlemi sırasında bir hata oluştu." });
    }

}