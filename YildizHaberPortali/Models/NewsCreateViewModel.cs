// Models/NewsCreateViewModel.cs

using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için
using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class NewsCreateViewModel
    {
        // Haber Modelinden Alınan Alanlar
        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik zorunludur.")]
        public string Content { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Yazar zorunludur.")]
        public string Author { get; set; }

        // İlişki Alanları (Foreign Key)
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        // Yayın tarihi otomatik doldurulacak, yazar da oturumdan gelebilir.
        // Şimdilik basit tutuyoruz.

        // Yardımcı Alanlar (Kategori Listesi)
        // Bu, Controller'dan View'a kategori seçeneklerini taşımak için kullanılır.
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}