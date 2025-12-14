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
        public int Priority { get; set; }

        public string ImageUrl { get; set; } 

        public DateTime PublishDate { get; set; } 

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public bool IsPublished { get; set; } = true; 

    }
}