using System;

namespace YildizHaberPortali.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.Now; 
        public bool IsApproved { get; set; } = true;

        public int NewsId { get; set; }
        public News News { get; set; }

        public string UserId { get; set; } 
        public AppUser User { get; set; }
        public string UserName { get; set; }
    }
}