using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YildizHaberPortali.Models.ViewModels
{
    public class NewsUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string? Author { get; set; }
        public string? ExistingImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsPublished { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}