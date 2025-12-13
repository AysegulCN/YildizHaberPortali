// Controllers/UserController.cs
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için

[Authorize(Roles = "Admin")] // Sadece Admin erişebilir
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: /User/Index (Tüm kullanıcıları listele)
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesViewModel>();

        foreach (IdentityUser user in users)
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
}