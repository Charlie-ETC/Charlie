using System.Collections.Generic;

namespace Charlie.Giphy
{
    public class Sticker
    {
        public string type;
        public string id;
        public string slug;
        public string url;
        public string bitlyGifUrl;
        public string bitlyUrl;
        public string embedUrl;
        public string username;
        public string source;
        public string rating;
        public string contentUrl;
        public string sourceTld;
        public string sourcePostUrl;
        public string isIndexable;
        public string importDatetime;
        public string trendingDatetime;
        public string title;

        public User user;
        public Dictionary<string, Image> images;
    }
}
