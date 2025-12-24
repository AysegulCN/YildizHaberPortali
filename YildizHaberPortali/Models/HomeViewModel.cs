using System.Collections.Generic;

namespace YildizHaberPortali.Models
{
    public class HomeViewModel
    {
        public List<News> SliderNews { get; set; }

        public List<News> LatestNews { get; set; }

        public List<Category> Categories { get; set; }

        public List<News> MostReadNews { get; set; }
    }
}