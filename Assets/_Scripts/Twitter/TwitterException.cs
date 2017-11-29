using System;
using UnityEngine.Networking;

namespace Charlie.Twitter
{
    public class TwitterException : Exception
    {
        public readonly UnityWebRequest Request;

        public TwitterException() : base()
        {
        }

        public TwitterException(string message) : base(message)
        {
        }

        public TwitterException(string message, UnityWebRequest request) : base(message)
        {
            Request = request;
        }
    }
}
