using System;
using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad boş bırakılamaz.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta adresi şarttır.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Email { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj alanı boş bırakılamaz.")]
        public string Message { get; set; }

        public string? PhotoPath { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}