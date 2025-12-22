using System;

namespace YildizHaberPortali.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } // 🚀 Content değil Text!
        public DateTime CreatedDate { get; set; } = DateTime.Now; // 🚀 CreatedDate!
        public bool IsApproved { get; set; } = true;

        public int NewsId { get; set; }
        public News News { get; set; }

        public string UserId { get; set; } // 🚀 UserId!
        public AppUser User { get; set; }
    }
}