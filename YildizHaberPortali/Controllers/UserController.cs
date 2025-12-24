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

        public UserController(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
        }

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı!" });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == id)
            {
                return Json(new { success = false, message = "Kendi yönetici hesabınızı silemezsiniz!" });
            }

            if (!string.IsNullOrEmpty(user.ProfilePicture) && user.ProfilePicture != "undraw_profile.svg")
            {
                string path = Path.Combine(_hostEnvironment.WebRootPath, "img", user.ProfilePicture);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Silme işlemi sırasında teknik bir hata oluştu." });
        }

        public async Task<IActionResult> Writers()
        {
            var users = await _userManager.Users.ToListAsync();
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
                IsActive = (user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now),
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = allRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var allRoles = _roleManager.Roles.ToList();
            model.Roles = allRoles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.Branch = model.Branch;

            if (model.ProfileImageFile != null)
            {
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

            if (!model.IsActive)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            else
                await _userManager.SetLockoutEndDateAsync(user, null);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(model.SelectedRole))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı bilgileri pırıl pırıl güncellendi!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(model);
        }
    }
}