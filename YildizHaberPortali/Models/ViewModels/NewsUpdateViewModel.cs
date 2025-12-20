using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations; // Bu satırı ekle

namespace YildizHaberPortali.Models.ViewModels
{
    public class NewsUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen bir başlık giriniz")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Haber içeriği boş olamaz")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Lütfen bir kategori seçiniz")] // FK hatasını bu önler
        public int CategoryId { get; set; }

        public string? Author { get; set; }
        public string? ExistingImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsPublished { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}