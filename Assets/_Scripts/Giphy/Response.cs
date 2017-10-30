namespace Charlie.Giphy
{
    public class Response<T>
    {
        public T data;
        public Pagination pagination;
        public Meta meta;

        public class Pagination
        {
            public int totalCount;
            public int count;
            public int offset;
        }

        public class Meta
        {
            public int status;
            public string msg;
            public string responseId;
        }
    }
}
