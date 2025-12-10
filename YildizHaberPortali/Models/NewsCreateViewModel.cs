// Models/NewsCreateViewModel.cs

using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem için
using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class NewsCreateViewModel
    {
        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik zorunludur.")]
        public string Content { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Yazar zorunludur.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }

        public int Id { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}