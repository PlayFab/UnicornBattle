namespace PlayFab.Sockets.Models
{
    using System;
    using PlayFab;
    using SharedModels;

    [Serializable]
    public class GetConnectionInfoRequest : PlayFabRequestCommon { }

    [Serializable]
    public class GetConnectionInfoResponse : PlayFabResultCommon
    {
        public string AccessToken = "";
        public string Url = "";
    }

    [Serializable]
    public class SubscribeRequest
    {
        public Topic Topic { get; set; }
    }

    [Serializable]
    public class UnsubscribeRequest
    {
        public Topic Topic { get; set; }
    }

    [Serializable]
    public class PlayFabMessageBase
    {
        public EntityKey PublisherEntity;
        public Topic Topic;
        public string OriginalId;
        public string OriginalTimestamp;
        public string PayloadJSON;
    }

    [Serializable]
    public class PlayFabNetworkMessage : PlayFabMessageBase
    {
        public T ReadMessage<T>() where T : PlayFabMessageBase
        {
            var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
            return json.DeserializeObject<T>(PayloadJSON);
        }
    }

    [Serializable]
    public class Topic
    {
        public EntityKey Entity { get; set; }
        public string EventNamespace { get; set; }
        public string EventName { get; set; }

        //public override string ToString()
        //{
        //    return $"{EventNamespace}-{EventName}-{Entity}";
        //}

        public override bool Equals(object topic)
        {
            var internalTopic = topic as Topic;
            if (internalTopic == null)
                return false;

            return internalTopic.EventNamespace == this.EventNamespace &&
                   internalTopic.EventName == this.EventName &&
                   internalTopic.Entity.Id == this.Entity.Id &&
                   internalTopic.Entity.Type == this.Entity.Type;
        }

        public override int GetHashCode()
        {
            return (this.Entity.Id +
                this.Entity.Type +
                this.EventNamespace +
                this.EventName).GetHashCode();
        }
    }

    public class EntityKey
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }

    [Serializable]
    public enum ErrorStates
    {
        Ok = 0,
        Retry30S = 30,
        Retry5M = 300,
        Retry10M = 600,
        Retry15M = 900,
        Cancelled = -1
    }

    public class ErrorStrings
    {
        public const string MissingConnectionInfo =
            "Attempted to create a HubConnection without ConnectionInfo. Make sure that is called and completed first.";
        public const string MustBeConnectedSubscribe = "You must be connected to subscribe";
        public const string MustBeConnectedUnSubscribe = "You must be connected to subscribe";

    }

    public class Endpoints
    {
        public const string GetConnectionInfo = "/PubSub/GetConnectionInfo";
    }
}
