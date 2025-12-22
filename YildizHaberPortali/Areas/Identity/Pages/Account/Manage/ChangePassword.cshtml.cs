using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using YildizHaberPortali.Models;


namespace YildizHaberPortali.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Mevcut şifrenizi girmeniz gerekiyor.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mevcut Şifre")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Yeni şifre belirlemelisiniz.")]
            [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Yeni Şifre")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Yeni Şifre (Tekrar)")]
            [Compare("NewPassword", ErrorMessage = "Yeni şifreler birbiriyle uyuşmuyor.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kullanıcı bulunamadı.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // --- SENİN İSTEDİĞİN ÖZEL KONTROL ---
            // Eski şifre ile yeni şifre aynıysa hata fırlatıyoruz.
            if (Input.OldPassword == Input.NewPassword)
            {
                ModelState.AddModelError(string.Empty, "Yeni şifreniz, eski şifrenizle aynı olamaz. Lütfen farklı bir şifre seçiniz.");
                return Page();
            }
            // -------------------------------------

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kullanıcı bulunamadı.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    // Hataları Türkçe'ye çevirerek gösterelim
                    if (error.Code == "PasswordMismatch")
                        ModelState.AddModelError(string.Empty, "Mevcut şifrenizi yanlış girdiniz.");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Kullanıcı şifresini başarıyla değiştirdi.");
            StatusMessage = "Şifreniz başarıyla güncellendi.";

            return RedirectToPage();
        }
    }
}