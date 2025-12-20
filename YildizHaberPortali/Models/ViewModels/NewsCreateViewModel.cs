using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace YildizHaberPortali.Models.ViewModels
{
    public class NewsCreateViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public string? Author { get; set; }
        public IFormFile? ImageFile { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}