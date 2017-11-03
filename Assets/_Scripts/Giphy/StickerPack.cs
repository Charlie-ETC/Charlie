using System.Collections.Generic;

namespace Charlie.Giphy
{
    public class StickerPack
    {
        public int id;
        public string displayName;
        public string slug;
        public string contentType;
        public string shortDisplayName;
        public string description;
        public string bannerImage;
        public string parent;
        public bool hasChildren;
        public User user;
        public Sticker featuredGif;
        public List<Tag> tags;
        public List<StickerPack> ancestors;

        public class Tag
        {
            public string tag;
            public int rank;
        }
    }
}
