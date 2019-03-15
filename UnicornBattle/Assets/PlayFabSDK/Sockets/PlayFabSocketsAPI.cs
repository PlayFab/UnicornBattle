namespace PlayFab.Sockets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using PlayFab.Sockets.Models;
    
    public class PlayFabSocketsAPI 
    {
        public static readonly OnConnectEvent OnConnected = new OnConnectEvent();
        public static readonly OnDisconnectEvent OnDisconnected  = new OnDisconnectEvent();
        public static readonly OnConnectionErrorEvent OnConnectionError = new OnConnectionErrorEvent();
        public static readonly OnRetryConnectionEvent OnRetryConnection = new OnRetryConnectionEvent();
        public static readonly OnTopicResubscriptionFailedEvent OnTopicResubscriptionFailed = new OnTopicResubscriptionFailedEvent();
        public static bool Debugging = false;
        
        public class OnConnectEvent : UnityEvent{}
        public class OnDisconnectEvent : UnityEvent{}
        public class OnConnectionErrorEvent : UnityEvent<PlayFabError>{}
        public class OnRetryConnectionEvent : UnityEvent<float,string>{}
        public class OnTopicResubscriptionFailedEvent : UnityEvent<Topic>{}


        private static PlayFabSocketsBehaviour _transport;
        
        /// <summary>
        /// Initialize the Sockets API, this will create a Monobehaviour in the scene
        /// You must call this before any other API usage. 
        /// </summary>
        /// <param name="autoConnect"> auto connect after initialization</param>
        public static void Initialize(bool autoConnect = false)
        {
            PlayFabSocketsBehaviour.OnConnected += OnInternalConnected;
            PlayFabSocketsBehaviour.OnDisconnected += OnInternalDisconnected;
            PlayFabSocketsBehaviour.OnConnectionError += OnInternalConnectionError;
            PlayFabSocketsBehaviour.OnRetryConnection += OnInternalRetryConnection;
            PlayFabSocketsBehaviour.OnTopicResubscriptionFailed += OnInternalTopicResubscribeFailed;
            
            PlayFabSocketsBehaviour.CreateInstance();
            _transport = PlayFabSocketsBehaviour.instance; 
            
            if (autoConnect)
            {
                Connect();
            }
            _transport.Debugging = Debugging;
        }

        /// <summary>
        /// Connect to the SignalR Hub, call after Initialize
        /// </summary>
        public static void Connect()
        {
            _transport.Initialize();
            _transport.Connect();
        }

        /// <summary>
        /// Disconnect from the SignalR Hub.
        /// Note: OnDisconnected event only fires when this is called, use OnConnectionError for connection faults
        /// </summary>
        public static void Disconnect()
        {
            _transport.Disconnect();
        }
        
        /// <summary>
        /// Overload for Connect,  Connect to the SignalR hub, with some event subscriptions
        /// </summary>
        /// <param name="onConnect">get notified when connected</param>
        /// <param name="onDisconnect">get notified when disconnected</param>
        /// <param name="onConnectionError">get notified when a connection fault occurs</param>
        public static void Connect(UnityAction onConnect, UnityAction onDisconnect, UnityAction<PlayFabError> onConnectionError)
        {
            OnConnected.AddListener(onConnect);
            OnDisconnected.AddListener(onDisconnect);
            OnConnectionError.AddListener(onConnectionError);
            Connect();
        }
        
        /// <summary>
        /// Subscribe to a PlayStream or Message topic
        /// </summary>
        /// <param name="topic">The topic you wish to subscribe to</param>
        /// <param name="successCallback">Fires if subscription was successful</param>
        /// <param name="exceptionCallback">Fires if the subscription was unsuccessful</param>
        public static void Subscribe(Topic topic, Action successCallback, Action<Exception> exceptionCallback)
        {
            _transport.Initialize();
            if (!_transport.IsConnected())
            {
                Debug.LogError(ErrorStrings.MustBeConnectedSubscribe);
                return;
            }
            _transport.Subscribe(topic, successCallback, exceptionCallback);
        }
        
        /// <summary>
        /// Subscribe to multiple PlayStream or Message topics at once
        /// </summary>
        /// <param name="topics">A list of topics you wish to subscribe to</param>
        /// <param name="successCallback">Fires upon success, returns a List of topics that were successfully subscribed to. pass null to omit event</param>
        /// <param name="exceptionCallback">Fires upon failure to subscribe, returns a list of Exceptions from failed subscriptions. pass null to omit event</param>
        public static void Subscribe(List<Topic> topics, Action<List<Topic>> successCallback, Action<List<Exception>> exceptionCallback)
        {
            _transport.Initialize();
            if (!_transport.IsConnected())
            {
                Debug.LogError(ErrorStrings.MustBeConnectedSubscribe);
                return;
            }
            _transport.Subscribe(topics,successCallback,exceptionCallback);
        }
        
        /// <summary>
        /// Unsubscribe from a topic
        /// </summary>
        /// <param name="topic">The topic you wish to unsubscribe from</param>
        /// <param name="unsubscribeComplete">Fires if unsubscription was successful, pass null to omit event</param>
        /// <param name="exceptionCallback">Fires if unsubscription was not successful, pass null to omit event</param>
        public static void Unsubscribe(Topic topic, Action unsubscribeComplete, Action<Exception> exceptionCallback)
        {
            _transport.Initialize();
            if (!_transport.IsConnected())
            {
                Debug.LogError(ErrorStrings.MustBeConnectedUnSubscribe);
                return;
            }
            _transport.Unsubscribe(topic,unsubscribeComplete,exceptionCallback);
        }
        
        /// <summary>
        /// Unsubscribe from multiple topics at onces
        /// </summary>
        /// <param name="topics">List of topics to unsubscribe from</param>
        /// <param name="unsubscribeComplete">List of topics you successfully unsubscribed from. pass null to omit event</param>
        /// <param name="exceptionCallback">List of exceptions from failed unsubscriptions. pass null to omit event</param>
        public static void Unsubscribe(List<Topic> topics, Action<List<Topic>> unsubscribeComplete,
            Action<List<Exception>> exceptionCallback)
        {
            _transport.Initialize();
            if (!_transport.IsConnected())
            {
                Debug.LogError(ErrorStrings.MustBeConnectedUnSubscribe);
                return;
            }
            _transport.Unsubscribe(topics,unsubscribeComplete,exceptionCallback);
        }
        
        /// <summary>
        /// Register a handler to receive messages on a topic
        /// </summary>
        /// <param name="topic">Topic you wish to receive a message about</param>
        /// <param name="handler">Function (Action) handler that can receive the message</param>
        public static void RegisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            _transport.Initialize();
            _transport.RegisterHandler(topic, handler);
        }

        /// <summary>
        /// Unregister a handler from receiving messages on a topic
        /// </summary>
        /// <param name="topic">Topic you no longer wish to handle messages for</param>
        /// <param name="handler">Original handler that you previously registered to handle the message</param>
        public static void UnRegisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            _transport.Initialize();
            _transport.UnregisterHandler(topic, handler);
        }

        /// <summary>
        /// Internal pass through for connected events
        /// </summary>
        private static void OnInternalConnected()
        {
            OnConnected.Invoke();
        }
        
        /// <summary>
        /// Internal pass through for disconnection events
        /// </summary>
        private static void OnInternalDisconnected()
        {
            OnDisconnected.Invoke();
        }

        /// <summary>
        /// Internal pass through for Connection error events
        /// </summary>
        /// <param name="error"></param>
        private static void OnInternalConnectionError(PlayFabError error)
        {
            OnConnectionError.Invoke(error);
        }

        /// <summary>
        /// Internal pass through for Connection retry events
        /// </summary>
        /// <param name="retryTime"></param>
        /// <param name="message"></param>
        private static void OnInternalRetryConnection(float retryTime, string message)
        {
            OnRetryConnection.Invoke(retryTime, message);
        }
        
        /// <summary>
        /// Internal pass through for topic re-subscription failures
        /// </summary>
        /// <param name="topic"></param>
        private static void OnInternalTopicResubscribeFailed(Topic topic)
        {
            OnTopicResubscriptionFailed.Invoke(topic);
        }
    }

}
