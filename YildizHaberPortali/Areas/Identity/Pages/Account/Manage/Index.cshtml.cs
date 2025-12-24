using System;
using System.ComponentModel.DataAnnotations;
using System.Linq; 
using System.Security.Claims; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YildizHaberPortali.Models;


namespace YildizHaberPortali.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
            [EmailAddress]
            [Display(Name = "E-posta Adresi")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Ad alanı gereklidir.")]
            [Display(Name = "Ad")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Soyad alanı gereklidir.")]
            [Display(Name = "Soyad")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Telefon Numarası")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            var claims = await _userManager.GetClaimsAsync(user);
            var fullNameClaim = claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

            string firstName = "";
            string lastName = "";

            if (!string.IsNullOrEmpty(fullNameClaim))
            {
                var names = fullNameClaim.Split(' ');
                if (names.Length > 1)
                {
                    lastName = names.Last();
                    firstName = string.Join(" ", names.Take(names.Length - 1));
                }
                else
                {
                    firstName = names[0];
                }
            }

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Kullanıcı bulunamadı.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Kullanıcı bulunamadı.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (setEmailResult.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, Input.Email);
                }
                else
                {
                    StatusMessage = "Hata: E-posta güncellenemedi.";
                    return RedirectToPage();
                }
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var oldClaim = claims.FirstOrDefault(c => c.Type == "FullName");
            var newFullName = $"{Input.FirstName} {Input.LastName}";

            if (oldClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, oldClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("FullName", newFullName));

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Profiliniz başarıyla güncellendi";
            return RedirectToPage();
        }
    }
}