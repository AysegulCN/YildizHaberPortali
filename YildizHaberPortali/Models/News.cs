// Models/News.cs

using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string ImageUrl { get; set; } // Boş olabilir

        public DateTime PublishDate { get; set; } // Controller'da otomatik atanıyor

        [Required]
        public string Author { get; set; }

        // İlişki (Foreign Key)
        [Required]
        public int CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; }

        // Models/News.cs
        public bool IsPublished { get; set; } = true; // Varsayılan olarak true yapın
    }
}