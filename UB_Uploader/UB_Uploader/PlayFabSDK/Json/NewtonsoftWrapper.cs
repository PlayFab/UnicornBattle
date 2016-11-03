#if USING_NEWTONSOFT
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace PlayFab.Json
{
    public class NewtonsofJsonInstance : ISerializer
    {
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            Converters = { new StringEnumConverter(), new TimeSpanFloatSeconds(), new IsoDateTimeConverter { DateTimeFormat = PlayFabUtil.DefaultDateTimeFormats[0] } },
        };
        private static Formatting JsonFormatting = Formatting.None;

        private readonly JsonSerializer _serializer = JsonSerializer.Create(JsonSettings);

        public T DeserializeObject<T>(string json)
        {
            return _serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        public T DeserializeObject<T>(string json, object jsonSerializerStrategy)
        {
            var customSerializer = JsonSerializer.Create((JsonSerializerSettings)jsonSerializerStrategy);
            return customSerializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        public object DeserializeObject(string json)
        {
            return _serializer.Deserialize(new JsonTextReader(new StringReader(json)));
        }

        public string SerializeObject(object json)
        {
            var jsonString = new StringWriter();
            var writer = new JsonTextWriter(jsonString) { Formatting = JsonFormatting };
            _serializer.Serialize(writer, json);
            return jsonString.ToString();
        }

        public string SerializeObject(object json, object jsonSerializerStrategy)
        {
            var customSerializer = JsonSerializer.Create((JsonSerializerSettings)jsonSerializerStrategy);
            var jsonString = new StringWriter();
            var writer = new JsonTextWriter(jsonString) { Formatting = JsonFormatting };
            customSerializer.Serialize(writer, json);
            return jsonString.ToString();
        }
    }

    public class TimeSpanFloatSeconds : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var timeSpan = (TimeSpan)value;
            serializer.Serialize(writer, timeSpan.TotalSeconds);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return TimeSpan.FromSeconds(serializer.Deserialize<float>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }
    }
}
#endif
