// Models/Category.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YildizHaberPortali.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Bu alan zorunlu değilse bile eklenmeli:
        public string Slug { get; set; }
    }
}