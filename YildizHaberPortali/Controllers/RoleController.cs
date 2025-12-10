// Controllers/RoleController.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için

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

    // POST: /Role/Create
    [HttpPost]
    public async Task<IActionResult> Create(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Rol oluşturulurken hata oluştu.");
        }
        return View((object)roleName); 
    }
}