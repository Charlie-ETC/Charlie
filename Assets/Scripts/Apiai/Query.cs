using System.Collections.Generic;

public class Query {
    public string query;
    public string v;
    public Event e;
    public string sessionId;
    public string lang;
    public Context[] contexts;
    public bool resetContexts;
    //public Entities[] entities;
    public string timezone;
    public Location location;
    //public object originalRequest;

    public class Event
    {
        public string name;
        public Dictionary<string, string> data;
    }

    public class Location
    {
        public double latitude;
        public double longitude;
    }
}
