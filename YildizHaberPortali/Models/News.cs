namespace YildizHaberPortali.Models
{
    public class News
    {
        public int Id { get; set; } // Primary Key
        public string Title { get; set; } // Haber Başlığı
        public string Content { get; set; } // Haber İçeriği
        public string ImageUrl { get; set; } // Kapak Resmi URL'si
        public DateTime PublishDate { get; set; } // Yayın Tarihi
        public string Author { get; set; } // Yazar

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property: Haberin ait olduğu kategori
        public Category Category { get; set; }
    }
}