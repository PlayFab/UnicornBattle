using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.Sockets;
using System;
using PlayFab.Sockets.Models;

public class PF_PubSub
{
    public delegate void PubSubMessageHandler(MessageFromServer msg);
    public static event PubSubMessageHandler OnMessageToPlayer;
    public static event PubSubMessageHandler OnMessageToAllPlayers;

    public static EntityKey currentEntity;

    private static bool isInitialized = false;

    public static void InitializePubSub()
    {
        if (isInitialized)
            return;

        isInitialized = true;
        //PlayFabSocketsAPI.Debugging = true;
        PlayFabSocketsAPI.Initialize();

        PlayFabSocketsAPI.OnConnected.AddListener(OnSocketsConnected);
        PlayFabSocketsAPI.Connect();
        PlayFabSocketsAPI.OnConnectionError.AddListener(OnSocketsConnectionError);
        PlayFabSocketsAPI.OnDisconnected.AddListener(OnSocketsDisconnected);


    }

    private static void OnSocketsConnected()
    {
        // create topics for both title specific AND player specific messages
        var entity = new PlayFab.Sockets.Models.EntityKey()
        {
            Id = currentEntity.Id,
            Type = currentEntity.Type
        };

        //Create a list of Topics to subscribe to
        var topics = new List<Topic>();

        //Create a Topic object to listen to
        var sendMessageToPlayerTopic= new Topic()
        {
            Entity = entity,
            EventName = "MessageToPlayer",
            EventNamespace = "com.playfab.events.unicornbattle"
        };
        //Add that topic to the array		
        topics.Add(sendMessageToPlayerTopic);

        PlayFabSocketsAPI.RegisterHandler(sendMessageToPlayerTopic, OnReceiveMessageToPlayer);

        var sendMessageToAllPlayersTopic= new Topic()
        {
            Entity = new PlayFab.Sockets.Models.EntityKey() {
                Type = "Title",
                Id = PlayFabSettings.TitleId
            },
            EventName = "MessageToAllPlayers",
            EventNamespace = "com.playfab.events.unicornbattle"
        };
        //Add that topic to the array		
        topics.Add(sendMessageToAllPlayersTopic);

        PlayFabSocketsAPI.RegisterHandler(sendMessageToAllPlayersTopic, OnReceiveMessageToPlayer);


        //send topic subscriptions and output any success or failures
        PlayFabSocketsAPI.Subscribe(topics, (subscribedTopics) =>
        {
            if (PlayFabSocketsAPI.Debugging)
                Debug.Log("Subscribe Success");
                
            subscribedTopics.ForEach((t) =>
            {
                if (PlayFabSocketsAPI.Debugging)
                    Debug.LogFormat("{0} Subscribed Successfully", t.EventName);
            });
        }, (subscribedErrors) =>
        {
            if (PlayFabSocketsAPI.Debugging)
                Debug.Log("Subscribe Failed");
            
            subscribedErrors.ForEach((t) =>
            {
                if (PlayFabSocketsAPI.Debugging)
                    Debug.LogFormat("{0}", t.Message);
            });			
        });

    }

    private static void OnSocketsDisconnected()
    {
        Debug.Log("PlayFab PubSub:You were disconnected from the server");
    }

    private static void OnSocketsConnectionError(PlayFabError error)
    {
        Debug.LogFormat("PlayFab PubSub Error: {0}", error.GenerateErrorReport());
    }

    private static void OnReceiveMessageToPlayer(PlayFabNetworkMessage netMsg) {
        Debug.Log(netMsg.PayloadJSON);		
        var jsonSerializer = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        if (netMsg.PayloadJSON != null) 
        {
            var msg = jsonSerializer.DeserializeObject<MessageFromServer>(netMsg.PayloadJSON);
            
            

            if (OnMessageToPlayer != null)                        
                OnMessageToPlayer(msg);            
        }
    }

    private static void ProcessMsgResponse(bool response)
    {
        
    }
}
