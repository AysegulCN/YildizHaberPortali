using System.Collections.Generic;

namespace YildizHaberPortali.Models
{
    public class HomeViewModel
    {
        // Ana sayfadaki büyük kayan haberler
        public List<News> SliderNews { get; set; }

        // Slider'ın yanındaki 4'lü küçük haber alanı
        public List<News> LatestNews { get; set; }

        // Blok blok dizeceğimiz kategoriler
        public List<Category> Categories { get; set; }

        // Sağ taraftaki "En Çok Okunanlar" listesi
        public List<News> MostReadNews { get; set; }
    }
}