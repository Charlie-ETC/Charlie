using System;
using UnityEngine.Networking;

namespace Charlie.Giphy
{
    public class GiphyException : Exception
    {
        public readonly UnityWebRequest Request;

        public GiphyException() : base()
        {
        }

        public GiphyException(string message) : base(message)
        {
        }

        public GiphyException(string message, UnityWebRequest request) : base(message)
        {
            Request = request;
        }
    }
}
