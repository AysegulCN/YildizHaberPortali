using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Models;
using YildizHaberPortali.Models.ViewModels;

[Authorize(Roles = "Admin")] // Sadece Admin erişebilir
public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /User/Index (Tüm kullanıcıları listele)
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesViewModel>();

        foreach (AppUser user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRolesViewModel.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            });
        }
        return View(userRolesViewModel);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Writers()
    {
        // 1. Sistemdeki tüm kullanıcıları çek
        var users = _userManager.Users.ToList();
        var writers = new List<AppUser>();

        foreach (var user in users)
        {
            // 2. Sadece "Yazar" rolünde olanları listeye ekle
            if (await _userManager.IsInRoleAsync(user, "Yazar"))
            {
                writers.Add(user);
            }
        }

        return View(writers);
    }

    [Authorize(Roles = "Admin")]
   
        // 🚀 DÜZENLEME SAYFASI (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                IsActive = !user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.Now,
                SelectedRole = userRoles.FirstOrDefault() ?? "Kullanıcı",
                Roles = allRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList()
            };

            return View(model);
        }

        // 🚀 GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // 1. Temel Bilgileri Güncelle
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;

            // 2. Hesap Dondurma / Aktifleştirme (Lockout Mantığı)
            if (!model.IsActive)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); // Sonsuza kadar dondur
            else
                await _userManager.SetLockoutEndDateAsync(user, null); // Kilidi aç

            var updateResult = await _userManager.UpdateAsync(user);

            // 3. Rol Değiştirme
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (updateResult.Succeeded)
            {
                TempData["Success"] = "Yazar bilgileri başarıyla güncellendi.";
                return RedirectToAction("Writers");
            }

            return View(model);
        }
    

    // GET: /User/ManageRoles?userId={id} (Rol Atama Formunu Getir)
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        ViewBag.UserName = user.UserName;

        var roles = await _roleManager.Roles.ToListAsync();
        var model = new List<ManageUserRolesViewModel>();

        foreach (var role in roles)
        {
            model.Add(new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
            });
        }
        return View(model);
    }

    // POST: /User/ManageRoles (Rolleri Kaydet)
    [HttpPost]
    public async Task<IActionResult> ManageRoles(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Mevcut rolleri temizle
        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Kullanıcı rolleri kaldırılamadı.");
            return View(model);
        }

        // Seçilen yeni rolleri ekle
        result = await _userManager.AddToRolesAsync(user,
            model.Where(x => x.IsSelected).Select(y => y.RoleName));

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Kullanıcıya rol eklenemedi.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // 1. Sayfayı Açan Metot (GET) - 404'ü bu çözer!
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user); // Kullanıcının mevcut rolleri
        var allRoles = _roleManager.Roles.ToList(); // Sistemdeki tüm roller

        var viewModel = new AssignRoleViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = allRoles.Select(role => new RoleSelection
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            }).ToList()
        };

        return View(viewModel);
    }

    // 2. Rolleri Kaydeden Metot (POST)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        // Önce tüm mevcut rolleri sil (En temiz yöntem)
        await _userManager.RemoveFromRolesAsync(user, userRoles);

        // Seçilen yeni rolleri ekle
        var selectedRoles = model.Roles.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();
        if (selectedRoles.Any())
        {
            await _userManager.AddToRolesAsync(user, selectedRoles);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAjax(string id)
    {
        // 1. Kullanıcıyı bul
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

        // 2. Güvenlik: Giriş yapmış olan admin kendini silemesin
        if (user.UserName == User.Identity.Name)
            return Json(new { success = false, message = "Kendi hesabınızı silemezsiniz!" });

        // 3. Kullanıcıyı sil
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Json(new { success = true });
        }

        return Json(new { success = false, message = "Silme işlemi sırasında bir hata oluştu." });
    }


}