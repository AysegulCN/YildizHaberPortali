using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı zorunludur.")]
        public string Name { get; set; }

        public string Slug { get; set; }
        public ICollection<News> News { get; set; }
    }
}
