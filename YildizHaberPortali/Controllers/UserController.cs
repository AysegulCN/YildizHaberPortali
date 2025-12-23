using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Models;
using YildizHaberPortali.Models.ViewModels;

namespace YildizHaberPortali.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        // 🚀 TEK VE TEMİZ CONSTRUCTOR
        public UserController(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
        }

        // 👥 KULLANICI LİSTESİ (Index)
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(userRolesViewModel);
        }

        // 🖋️ YAZAR KADROSU (Writers)
        public async Task<IActionResult> Writers()
        {
            var users = _userManager.Users.ToList();
            var writers = new List<AppUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Yazar"))
                {
                    writers.Add(user);
                }
            }
            return View(writers);
        }

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
                Bio = user.Bio,
                Branch = user.Branch,
                ExistingProfilePicture = user.ProfilePicture,
                // LockoutEnd gelecekteyse hesap dondurulmuş demektir.
                IsActive = (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now),
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = allRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList()
            };
            return View(model);
        }

        // Düzenleme İşlemi (POST) - PRO VERSİYON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            // 🚀 Formun içindeki Roller listesini tekrar doldurmalıyız (Validation hatası olursa sayfa boş gelmesin diye)
            var allRoles = _roleManager.Roles.ToList();
            model.Roles = allRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // 1. Temel Bilgileri Güncelle
            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.Branch = model.Branch;

            // 2. Profil Resmi Yönetimi
            if (model.ProfileImageFile != null)
            {
                // Eski resmi sil (Default resim değilse)
                if (!string.IsNullOrEmpty(user.ProfilePicture) && user.ProfilePicture != "undraw_profile.svg")
                {
                    string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", user.ProfilePicture);
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImageFile.FileName;
                string newPath = Path.Combine(_hostEnvironment.WebRootPath, "img", uniqueFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }
                user.ProfilePicture = uniqueFileName;
            }

            // 3. Hesap Aktiflik Durumu (Lockout Mantığı)
            if (!model.IsActive)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); // Sonsuza kadar dondur
            else
                await _userManager.SetLockoutEndDateAsync(user, null); // Kilidi aç

            // 4. Rol Güncelleme (Pro Dokunuş)
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.SelectedRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Yazar bilgileri pırıl pırıl güncellendi!";
                return RedirectToAction("Writers");
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }

        // Diğer metodların (DeleteAjax, Index vb.) aynen devam edebilir...
    }
}