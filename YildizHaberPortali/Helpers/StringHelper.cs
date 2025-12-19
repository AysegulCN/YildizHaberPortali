namespace YildizHaberPortali.Helpers
{
    public static class StringHelper
    {
        public static string ToSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = text.ToLower();
            text = text.Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ö", "o").Replace("ç", "c");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Replace(" ", "-");
            return text;
        }
    }
}