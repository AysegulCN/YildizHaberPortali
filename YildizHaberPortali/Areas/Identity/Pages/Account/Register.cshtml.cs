using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims; // <--- BU EKLENDİ (İsim kaydı için)
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YildizHaberPortali.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            // --- YENİ EKLENEN ALANLAR ---
            [Required(ErrorMessage = "Ad alanı zorunludur.")]
            [Display(Name = "Ad")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Soyad alanı zorunludur.")]
            [Display(Name = "Soyad")]
            public string LastName { get; set; }
            // ---------------------------

            [Required(ErrorMessage = "E-posta gereklidir.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre gereklidir.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Şifre Tekrar")]
            [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // --- İSİM VE SOYADI SİSTEME EKLEME (CLAIM OLARAK) ---
                    // Bu yöntem veritabanı değişikliği gerektirmez, çok güvenlidir.
                    await _userManager.AddClaimAsync(user, new Claim("FullName", $"{Input.FirstName} {Input.LastName}"));

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // --- BİLDİRİM MESAJI (TempData) ---
                    TempData["SuccessMessage"] = $"Aramıza hoşgeldin, {Input.FirstName}!";

                    // --- YÖNLENDİRME DÜZELTMESİ ---
                    // Artık direkt Home Controller'ın Index sayfasına (Ana Vitrin) gidiyor.
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}