namespace AppCenterEditor
{
    public interface ISerializer
    {
        T DeserializeObject<T>(string json);
        T DeserializeObject<T>(string json, object jsonSerializerStrategy);
        object DeserializeObject(string json);

        string SerializeObject(object json);
        string SerializeObject(object json, object jsonSerializerStrategy);
    }
}
