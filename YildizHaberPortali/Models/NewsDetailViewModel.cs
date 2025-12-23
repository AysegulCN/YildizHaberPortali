using System.Collections.Generic;

namespace YildizHaberPortali.Models
{
    public class NewsDetailViewModel
    {
        public News News { get; set; }
        public List<Comment> Comments { get; set; }
        public Comment NewComment { get; set; }
    }
}