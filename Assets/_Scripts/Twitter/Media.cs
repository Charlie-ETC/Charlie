using System.Collections.Generic;

public class Media
{
    public long mediaId;
    public string mediaIdString;
    public int size;
    public int expiresAfterSec;

    public class Image
    {
        public string imageType;
        public int w;
        public int h;
    }
}
