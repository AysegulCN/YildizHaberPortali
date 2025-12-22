using Microsoft.AspNetCore.Mvc.Rendering; // 🚀 SelectListItem hatasını (CS0246) bu çözer
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models.ViewModels // 🚀 Namespace'in doğruluğundan emin ol
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        [Required] public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public string SelectedRole { get; set; }
        public List<SelectListItem>? Roles { get; set; } // 🚀 CS0246 burada düzelecek
    }
}