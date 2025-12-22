using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string FullName { get; set; }

        public string Email { get; set; } 

        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler birbiriyle uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ExistingPicture { get; set; }

    }
}