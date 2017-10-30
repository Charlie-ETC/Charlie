using System.Collections.Generic;

namespace Charlie.Unsplash
{
    public class Response
    {
        public int total;
        public int totalPages;
        public Photo[] results;

        public class Photo
        {
            public string id;
            public string createdAt;
            public int width;
            public int height;
            public string color;
            public int likes;
            public bool likedByUser;
            public string description;
            //public User user;
            //public Collection[] currentUserCollections;
            public Dictionary<string, string> urls;
            public Dictionary<string, string> links;
        }
    }
}
