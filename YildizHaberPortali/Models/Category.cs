namespace YildizHaberPortali.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Kategori Adı
        public string Slug { get; set; } // SEO Dostu URL için (örn: "teknoloji-haberleri")

        // Navigation Property: Bu kategoriye ait haberleri tutacak
        public ICollection<News> News { get; set; }
    }
}