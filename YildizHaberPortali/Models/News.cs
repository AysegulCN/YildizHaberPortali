// Models/News.cs

using System.ComponentModel.DataAnnotations;

namespace YildizHaberPortali.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

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

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}