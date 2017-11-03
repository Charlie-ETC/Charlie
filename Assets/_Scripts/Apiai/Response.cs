using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Charlie.Apiai
{
    [System.Serializable]
    public class Response
    {
        public string id;
        public string timestamp;
        public string lang;
        public Result result;

        public class ParameterConverter : CustomCreationConverter<Dictionary<string, object>>
        {
            public override Dictionary<string, object> Create(Type objectType)
            {
                return new Dictionary<string, object>();
            }

            public override bool CanConvert(Type objectType)
            {
                // in addition to handling IDictionary<string, object>
                // we want to handle the deserialization of dict value
                // which is of type object
                return objectType == typeof(object) || base.CanConvert(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartObject
                    || reader.TokenType == JsonToken.Null)
                    return base.ReadJson(reader, objectType, existingValue, serializer);

                // if the next token is not an object
                // then fall back on standard deserializer (strings, numbers etc.)
                return serializer.Deserialize(reader);
            }
        }

        public class Result
        {
            public string source;
            public string resolvedQuery;
            public string action;
            public bool actionIncomplete;

            [JsonProperty("parameters")]
            [JsonConverter(typeof(ParameterConverter))]
            public Dictionary<string, object> parameters;

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
}
