namespace PlayFab.Sockets.Models
{
    using System;
    using System.Collections.Generic;
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
    public class PlayFabNetworkMessage
    {
        public Event @event { get; set; }
        public T ReadMessage<T>()
        {
            var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
            return json.DeserializeObject<T>(@event.payload);
        }
    }

    [Serializable]
    public class Topic
    {
        public EntityKey Entity { get; set; }
        public EventFullName FullName { get; set; }

        //public override string ToString()
        //{
        //    return $"{EventNamespace}-{EventName}-{Entity}";
        //}

        public override bool Equals(object topic)
        {
            var internalTopic = topic as Topic;
            if (internalTopic == null)
                return false;

            return internalTopic.FullName.Namespace == this.FullName.Namespace &&
                   internalTopic.FullName.Name == this.FullName.Name &&
                   internalTopic.Entity.Id == this.Entity.Id &&
                   internalTopic.Entity.Type == this.Entity.Type;
        }

        public override int GetHashCode()
        {
            return (this.Entity.Id +
                this.Entity.Type +
                this.FullName.Namespace +
                this.FullName.Name).GetHashCode();
        }
    }

    public class EntityKey
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }
    public class PubSubResponse
    {
        /// <summary>
        /// HTTP Response Int
        /// </summary>
        public int code;

        public object content;
    }

    public class PubSubJsonError
    {
        /// <summary>
        /// HTTP Response Int
        /// </summary>
        public int code;

        /// <summary>
        /// HTTP Response Name
        /// </summary>
        public string status;

        public string error;

        /// <summary>
        /// GenericErrorCodes Int
        /// </summary>
        /// won't be used, mainly for server side consistency
        public int errorCode;

        /// <summary>
        /// GenericErrorCodes Name
        /// </summary>
        /// won't be used, mainly for server side consistency
        public string errorMessage;

        /// won't be used, mainly for server side consistency
        public string errorHash;

        // mapping of parameter name to error messages
        public Dictionary<string, string[]> errorDetails = null;
    }

    public class PubSubJsonSuccess<TResponseType> where TResponseType : class
    {
        /// <summary>
        /// HTTP Response Int
        /// </summary>
        public int code;

        /// <summary>
        /// HTTP Response Name
        /// </summary>
        public string status;

        public TResponseType data;
    }

    public class PubSubServiceException : Exception
    {
        public string[] requestIds;
        public PubSubServiceException(string message, string[] requestIds) 
            : base(message)
        {
            this.requestIds = requestIds;
        }
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


    [Serializable]
    public class Event
    {
        public const string CurrentSchemaVersion = "2.0.1";

        /// <summary>
        /// PlayStream event format version, following a semantic versioning scheme.
        /// This covers only the format of the common event structure as defined here, 
        /// not the payload formats of specific events.
        /// </summary>
        public string schemaVersion { get; set; }

        /// <summary>
        /// Combination of a namespace and name, which fully specify the event type.
        /// </summary>
        public EventFullName fullName { get; set; }

        /// <summary>
        /// Globally unique identifier for the event. Assigned by PlayFab when first generated or received.
        /// E.g. A0D1FE18136243E8BAE86BC76C5AD5EA
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// UTC time that the event was written to PlayStream.
        /// For events originating outside of PlayFab, such as those posted to the WriteEvents API, 
        /// this is the server time when PlayFab received the event.
        /// </summary>
        public DateTime timestamp { get; set; }

        /// <summary>
        /// The primary entity associated with the event.
        /// </summary>
        public EntityKey entity { get; set; }

        /// <summary>
        /// Entity that initiated the event.
        /// Matches the Owner entity when an entity updates a resource in its own profile.
        /// </summary>
        public EntityKey originator { get; set; }

        /// <summary>
        /// An optional field containing info about events originating outside of PlayFab.
        /// </summary>
        public OriginInfo originInfo { get; set; }

        /// <summary>
        /// Arbitrary data associated with the event.
        /// The format of the string is indicated by PayloadContentType. 
        /// The schema of the payload data varies depending on the FullName of event and is not 
        /// necessarily known by PlayFab. It may contain arbitrary JSON with nested objects and arrays,
        /// Base64 encoded Protocol Buffer binary serialized objects, etc.
        /// </summary>
        public string payload { get; set; }

        /// <summary>
        /// A property bag of lineage information about the primary entity associated with this event.
        /// The keys are entity types e.g. namespace, title. The values are the corresponding Id's e.g. namespace Id, title Id.
        /// </summary>
        public EntityLineage entityLineage { get; set; }

        /// <summary>
        /// Media Type of the Payload
        /// 
        /// Examples:
        /// 'JSON' (default)
        /// 'Base64' (binary, e.g. Protocol Buffers)
        /// </summary>
        public PayloadContentType payloadContentType { get; set; }

        public static string GenerateEventId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    public static class EventExtensions
    {
        public static Topic GetTopic(this Event @event)
        {
            return new Topic()
            {
                Entity = @event.entity,
                FullName = @event.fullName
            };
        }
    }

    [Serializable]
    public class EventFullName
    {
        /// <summary>
        /// Namespace of the event, which scopes the meaning of the Name. The first part of the Namespace
        /// is a standard namespace.  For custom events this is always "custom". Optionally, this 
        /// top-level namespace may be followed by additional '.' delimited values to form a sub-categorization.
        /// E.g. "com.playfab.events.files" or "org.example.schema1".
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Name of the event, unique within the Namespace, indicating the "type" or "meaning" of the event
        /// as well as the format of its payload, if any. E.g. "file_updated"
        /// </summary>
        public string Name { get; set; }
        
        /*
        public EventFullName(string ns, string name)
        {
            Namespace = ns;
            Name = name;
        }*/

        public override string ToString()
        {
            return $"{Namespace}.{Name}";
        }
    }

    [Serializable]
    public class OriginInfo
    {
        /// <summary>	
        /// An optional identifier assigned to events originating outside of PlayFab.	
        /// For example, a client might assign IDs to events locally, before it posts them to the WriteEvents API.
        /// It is recommended that values of this property be globally unique, but uniqueness is not
        /// enforced by PlayFab.
        /// </summary>
        public string Id { get; set; }

        /// <summary>	
        /// An optional UTC timestamp assigned to events originating outside of PlayFab.	
        /// For example, a client might record timestamps for events locally, before it posts them to the WriteEvents API.	
        /// </summary>	
        public DateTime Timestamp { get; set; }

        ///// <summary>
        ///// Optional name/value pairs supplied by caller of the API which generated the event.
        ///// This can be used to capture details such as client version, flight, etc.
        ///// </summary>
        ////[StringDictionaryCountAndLength(5, 25, 25)]
        //[ProtoMember(9)]
        //public Dictionary<string, string> Tags { get; set; }

        public OriginInfo() { }

        public OriginInfo(string id, DateTime timestamp)
        {
            Id = id;
            Timestamp = timestamp;
        }
    }

    [Serializable]
    public enum PayloadContentType
    {
        JSON,
        Base64,
    }

    [Serializable]
    public sealed class EntityLineage : Dictionary<string, string>
    {
        public EntityLineage(params EntityKey[] ancestors)
            : base(ancestors.Length)
        {
            foreach (EntityKey ancestor in ancestors)
            {
                Add(ancestor.Type, ancestor.Id);
            }
        }

        public EntityLineage() { }
    }

}
