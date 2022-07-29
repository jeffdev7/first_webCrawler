using System.Diagnostics;

namespace first_web_crawler
{
    [DebuggerDisplay("{Model}, {Price}")]
    public class Car
    {
        public string Model { get; set; }
        public string Price { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
    }
}
