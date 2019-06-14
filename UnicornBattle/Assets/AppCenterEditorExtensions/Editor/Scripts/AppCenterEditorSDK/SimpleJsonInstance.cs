using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppCenterEditor
{
    public class SimpleJsonInstance : ISerializer
    {
        public T DeserializeObject<T>(string json)
        {
            return AppCenterSimpleJson.DeserializeObject<T>(json);
        }

        public T DeserializeObject<T>(string json, object jsonSerializerStrategy)
        {
            return AppCenterSimpleJson.DeserializeObject<T>(json, (IJsonSerializerStrategy)jsonSerializerStrategy);
        }

        public object DeserializeObject(string json)
        {
            return AppCenterSimpleJson.DeserializeObject(json);
        }

        public string SerializeObject(object json)
        {
            return AppCenterSimpleJson.SerializeObject(json);
        }

        public string SerializeObject(object json, object jsonSerializerStrategy)
        {
            return AppCenterSimpleJson.SerializeObject(json, (IJsonSerializerStrategy)jsonSerializerStrategy);
        }
    }
}
