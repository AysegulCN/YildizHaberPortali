using Microsoft.AspNetCore.Identity;

namespace YildizHaberPortali.Models
{
	// DİKKAT: AppUser : IdentityUser olmalı. Kendi isminden miras alamaz!
	public class AppUser : IdentityUser
	{
		public string FullName { get; set; }
		public string? Bio { get; set; }
		public string? ProfilePicture { get; set; }
        public string Branch { get; set; } // 🚀 Uzmanlık Dalı (Örn: Teknoloji, Ekonomi)
    }
}