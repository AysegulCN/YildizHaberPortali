using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Lütfen bir rol adı giriniz.")]
        [Display(Name = "Rol Adı")]
        public string RoleName { get; set; }
    }
}