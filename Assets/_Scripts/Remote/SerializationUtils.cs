using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Charlie.Remote
{
    public class SerializationUtils
    {
        public static string Serialize(Vector3 obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(obj.x);
                writer.WritePropertyName("y");
                writer.WriteValue(obj.y);
                writer.WritePropertyName("z");
                writer.WriteValue(obj.z);
                writer.WriteEndObject();
            }
            return stringBuilder.ToString();
        }

        public static string Serialize(Vector2 obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(obj.x);
                writer.WritePropertyName("y");
                writer.WriteValue(obj.y);
                writer.WriteEndObject();
            }
            return stringBuilder.ToString();
        }

        public static string Serialize(Transform obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                writer.WriteStartObject();
                writer.WritePropertyName("childCount");
                writer.WriteValue(obj.childCount);
                writer.WritePropertyName("eulerAngles");
                writer.WriteRawValue(Serialize(obj.eulerAngles));
                writer.WritePropertyName("forward");
                writer.WriteRawValue(Serialize(obj.forward));
                writer.WritePropertyName("hasChanged");
                writer.WriteValue(obj.hasChanged);
                writer.WritePropertyName("hierarchyCapacity");
                writer.WriteValue(obj.hierarchyCapacity);
                writer.WritePropertyName("hierarchyCount");
                writer.WriteValue(obj.hierarchyCount);
                writer.WritePropertyName("localEulerAngles");
                writer.WriteRawValue(Serialize(obj.localEulerAngles));
                writer.WritePropertyName("localPosition");
                writer.WriteRawValue(Serialize(obj.localPosition));
                writer.WritePropertyName("localScale");
                writer.WriteRawValue(Serialize(obj.localScale));
                writer.WritePropertyName("lossyScale");
                writer.WriteRawValue(Serialize(obj.lossyScale));
                writer.WritePropertyName("name");
                writer.WriteValue(obj.name);
                writer.WritePropertyName("position");
                writer.WriteRawValue(Serialize(obj.position));
                writer.WritePropertyName("right");
                writer.WriteRawValue(Serialize(obj.right));
                writer.WritePropertyName("tag");
                writer.WriteValue(obj.tag);
                writer.WritePropertyName("up");
                writer.WriteRawValue(Serialize(obj.up));
                writer.WriteEndObject();
            }
            return stringBuilder.ToString();
        }

        public static string Serialize(GameObject obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter writer = new JsonTextWriter(stringWriter)) {
                writer.WriteStartObject();
                writer.WritePropertyName("activeInHierarchy");
                writer.WriteValue(obj.activeInHierarchy);
                writer.WritePropertyName("activeSelf");
                writer.WriteValue(obj.activeSelf);
                writer.WritePropertyName("isStatic");
                writer.WriteValue(obj.isStatic);
                writer.WritePropertyName("layer");
                writer.WriteValue(obj.layer);
                writer.WritePropertyName("name");
                writer.WriteValue(obj.name);
                writer.WritePropertyName("tag");
                writer.WriteValue(obj.tag);
                writer.WritePropertyName("transform");
                writer.WriteRawValue(Serialize(obj.transform));
                writer.WriteEndObject();
            }
            return stringBuilder.ToString();
        }
    }
}
