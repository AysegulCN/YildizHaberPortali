// Models/Comment.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YildizHaberPortali.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NewsId { get; set; } // Hangi habere ait olduğu

        [Required]
        [StringLength(100)]
        public string AuthorName { get; set; } // Yorum yapanın adı

        [Required]
        public string Content { get; set; } // Yorum içeriği

        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; } = DateTime.Now;

        // İlişki (Navigation Property)
        [ForeignKey("NewsId")]
        public News News { get; set; }
    }
}