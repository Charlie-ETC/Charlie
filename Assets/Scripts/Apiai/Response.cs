using System.Collections.Generic;

public class Response {
    public string id;
    public string timestamp;
    public string lang;
    public Result result;

    public class Result
    {
        public string source;
        public string resolvedQuery;
        public string action;
        public bool actionIncomplete;
        public Dictionary<string, string> parameters;
        public Context[] contexts;
        public string speech;
        //public object messageObjects;
        public float score;
        public Metadata metadata;
        public Status status;
        public string sessionId;

        public class Metadata
        {
            public string intentId;
            public string webhookUsed;
            public string webhookForSlotFillingUsed;
            public float webhookResponseTime;
            public string intentName;
        }

        public class Status
        {
            public int code;
            public string errorType;
            public string errorId;
            public string errorDetails;
        }
    }
}
