using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Rol Adı")]
        public string RoleName { get; set; }
    }
}