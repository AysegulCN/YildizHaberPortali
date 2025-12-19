namespace YildizHaberPortali.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } // 'Name' yerine 'AuthorName'
        public string Content { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false; // Hata veren yer burasıydı

        public int NewsId { get; set; }
        public virtual News News { get; set; }
    }
}