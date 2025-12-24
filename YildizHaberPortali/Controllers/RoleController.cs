
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Models;

[Authorize(Roles = "Admin")] 
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAjax(string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
            return Json(new { success = false, message = "Rol adı boş olamaz!" });

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
        {
            var newRole = await _roleManager.FindByNameAsync(roleName);
            return Json(new { success = true, id = newRole.Id, name = newRole.Name });
        }
        return Json(new { success = false, message = "Bu rol zaten mevcut olabilir." });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAjax(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return Json(new { success = false });

        var result = await _roleManager.DeleteAsync(role);
        return Json(new { success = result.Succeeded });
    }

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

            if (result.Succeeded)
            {
               
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            return Json(new { success = false, message = "Rol bulunamadı!" });
        }

        if (role.Name == "Admin")
        {
            return Json(new { success = false, message = "Ana Yönetici (Admin) rolü silinemez!" });
        }

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Rol başarıyla silindi." });
        }

        return Json(new { success = false, message = "Silme işlemi sırasında bir hata oluştu." });
    }

}