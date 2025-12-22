using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models.ViewModels
{
    public class NewsCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen bir haber başlığı giriniz.")]
        [MinLength(5, ErrorMessage = "Başlık en az 5 karakter olmalıdır.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Haber içeriği boş bırakılamaz.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}