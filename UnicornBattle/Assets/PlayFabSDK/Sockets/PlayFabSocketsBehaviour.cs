namespace PlayFab.Sockets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using PlayFab;
    using PlayFab.Internal;
    using PlayFab.Sockets.Models;
    
    public class PlayFabSocketsBehaviour : SingletonMonoBehaviour<PlayFabSocketsBehaviour>
    {
        //Delegates for events fired from the service
        public delegate void OnConnectedEvent();
        public delegate void OnDisconnectedEvent();
        public delegate void OnConnectionErrorEvent(PlayFabError error);
        public delegate void OnRetryConnectionEvent(float retryTime, string message);
        public delegate void OnTopicResubscriptionFailedEvent(Topic topic);
        
        //Delegated events that you can subscribe to recieve notification from the service 
        public static event OnConnectedEvent OnConnected;
        public static event OnDisconnectedEvent OnDisconnected;
        public static event OnConnectionErrorEvent OnConnectionError;
        public static event OnRetryConnectionEvent OnRetryConnection;
        public static event OnTopicResubscriptionFailedEvent OnTopicResubscriptionFailed;

        //Show debug messages in the console
        public bool Debugging = false;
        
        /// <summary>
        /// Reference to the PlayFab SignalR Service
        /// </summary>
        private static PlayFabSignalRService _service;
        
        /// <summary>
        /// Connection error state machine for handling connection retries
        /// </summary>
        private static ErrorStates _CurrentErrorState = ErrorStates.Ok;
        
        /// <summary>
        /// Timer for connection retries
        /// </summary>
        private float _Timer = 0f;

        /// <summary>
        /// Added to connection retries to ensure users won't attempt connections at the same time
        /// </summary>
        private float _Jitter = 0f;
        
        /// <summary>
        /// Max Retry value, persisted from Enum values
        /// </summary>
        private float _RetryMax = (float) ErrorStates.Ok;
        
        /// <summary>
        /// Enum to handle connection state
        /// </summary>
        public enum ConnectionStates
        {
            Disconnected,
            Connecting,
            Connected,
            ConnectionFailed
        }

        /// <summary>
        /// State machine for connection to SignalR Hub
        /// </summary>
        private ConnectionStates _currentState = ConnectionStates.Disconnected;
        
        /// <summary>
        /// Maintained list of currently subscribed topics so we can re-subscribe on re-connection
        /// </summary>
        private readonly List<Topic> _currentlySubscribed = new List<Topic>();
        
        /// <summary>
        /// Internal variable to ensure we only initialize this compoenent once.
        /// </summary>
        private bool initializeComplete = false;

        /// <summary>
        /// Initialize this Monobehaviour object, it will create an instance of this object in the scene.
        /// Note: this object persists between scene loads (Always!),
        /// </summary>
        public void InitializeBehaviour()
        {
            if (initializeComplete) return;
            _service = new PlayFabSignalRService();
            _service.OnConnect += OnInternalConnected;
            _service.OnDisconnect += OnInternalDisconnected;
            _service.OnConnectionError += OnInternalConnectionError;
            _service.Debugging = Debugging;
            Application.runInBackground = true;
            CreateInstance(); //it's okay if this get's called multiple times, it handles singleton internally.
            initializeComplete = true;
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Check our connected state
        /// </summary>
        /// <returns>Returns a boolean true if connected.</returns>
        public bool IsConnected()
        {
            return _currentState == ConnectionStates.Connected;
        }
        
        /// <summary>
        /// Uses SignalR service to connect to the Hub
        /// OnConnected fires upon success
        /// </summary>
        public void Connect()
        {
            //Only attempt to connect if we are not in a connected state.
            if (_currentState == ConnectionStates.Disconnected && _currentState != ConnectionStates.ConnectionFailed)
            {
                _service.Connect();
            }
        }

        /// <summary>
        /// Disconnect from the SignalR Hub and unsubscribe from events.
        /// Note: Monobehaviour GameObject persists even when disconnected.
        /// </summary>
        public void Disconnect()
        {
            initializeComplete = false;
            _currentState = ConnectionStates.Disconnected;
            if (_service != null)
            {
                _service.OnConnect -= OnInternalConnected;
                _service.OnConnectionError -= OnInternalConnectionError;
                _service.Disconnect();
                _service.OnDisconnect -= OnInternalDisconnected;
            }
        }
        
        /// <summary>
        /// Subscribe to a PlayStream or Message topic
        /// </summary>
        /// <param name="topic">The topic you wish to subscribe to</param>
        /// <param name="successCallback">Fires if subscription was successful</param>
        /// <param name="exceptionCallback">Fires if the subscription was unsuccessful</param>
        public void Subscribe(Topic topic, Action successCallback, Action<Exception> exceptionCallback)
        {
            _service.Subscribe(topic, () =>
            {
                _currentlySubscribed.Add(topic);
                successCallback?.Invoke();
            }, exceptionCallback);
        }

        /// <summary>
        /// Subscribe to a PlayStream or Message topic
        /// </summary>
        /// <param name="topic">The topic you wish to subscribe to</param>
        /// <param name="successCallback">Fires if subscription was successful</param>
        /// <param name="exceptionCallback">Fires if the subscription was unsuccessful</param>
        public void Subscribe(Topic topic, Action<Topic> successCallback, Action<Exception> exceptionCallback)
        {
            _service.Subscribe(topic, () =>
            {
                _currentlySubscribed.Add(topic);
                successCallback?.Invoke(topic);
            }, exceptionCallback);
        }

        /// <summary>
        /// Subscribe to multiple PlayStream or Message topics at once
        /// </summary>
        /// <param name="topics">A list of topics you wish to subscribe to</param>
        /// <param name="successCallback">Fires upon success, returns a List of topics that were successfully subscribed to. pass null to omit event</param>
        /// <param name="exceptionCallback">Fires upon failure to subscribe, returns a list of Exceptions from failed subscriptions. pass null to omit event</param>
        public void Subscribe(List<Topic> topics, Action<List<Topic>> successCallback, Action<List<Exception>> exceptionCallback)
        {
            _service.Subscribe(topics, (subscribedTopics) =>
            {
                subscribedTopics.ForEach((t) =>
                {
                    if (_currentlySubscribed.Contains(t)) return;
                    _currentlySubscribed.Add(t);
                });
                
                successCallback?.Invoke(subscribedTopics);
            }, exceptionCallback);
        }

        /// <summary>
        /// Unsubscribe from a topic
        /// </summary>
        /// <param name="topic">The topic you wish to unsubscribe from</param>
        /// <param name="unsubscribeComplete">Fires if unsubscription was successful, pass null to omit event</param>
        /// <param name="exceptionCallback">Fires if unsubscription was not successful, pass null to omit event</param>
        public void Unsubscribe(Topic topic, Action unsubscribeComplete, Action<Exception> exceptionCallback)
        {
            _service.Unsubscribe(topic, () =>
            {
                var removeT = _currentlySubscribed.Find(t=>t.GetHashCode() == topic.GetHashCode());
                if (removeT != null)
                {
                    _currentlySubscribed.Remove(removeT);
                }
                unsubscribeComplete?.Invoke();
            }, exceptionCallback);
        }

        /// <summary>
        /// Unsubscribe from multiple topics at onces
        /// </summary>
        /// <param name="topics">List of topics to unsubscribe from</param>
        /// <param name="unsubscribeComplete">List of topics you successfully unsubscribed from. pass null to omit event</param>
        /// <param name="exceptionCallback">List of exceptions from failed unsubscriptions. pass null to omit event</param>
        public void Unsubscribe(List<Topic> topics, Action<List<Topic>> unsubscribeComplete,
            Action<List<Exception>> exceptionCallback)
        {
            _service.Unsubscribe(topics, (unSubscribedTopics) =>
            {
                unSubscribedTopics.ForEach((removedT) =>
                {
                    var removeT = _currentlySubscribed.Find(t=>t.GetHashCode() == removedT.GetHashCode());
                    if (removeT != null)
                    {
                        _currentlySubscribed.Remove(removeT);
                    }
                });
                unsubscribeComplete?.Invoke(unSubscribedTopics);
            },exceptionCallback);
        }
        
        /// <summary>
        /// Register a handler to receive messages on a topic
        /// </summary>
        /// <param name="topic">Topic you wish to receive a message about</param>
        /// <param name="handler">Function (Action) handler that can receive the message</param>
        public void RegisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            _service.RegisterHandler(topic, handler);
        }

        /// <summary>
        /// Unregister a handler from receiving messages on a topic
        /// </summary>
        /// <param name="topic">Topic you no longer wish to handle messages for</param>
        /// <param name="handler">Original handler that you previously registered to handle the message</param>
        public void UnregisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            _service.UnregisterHandler(topic, handler);
        }

        /// <summary>
        /// Internally handled event for when a connection or reconnection is made
        /// re-subscribes to topics on retry success
        /// </summary>
        private void OnInternalConnected()
        {
            _currentState = ConnectionStates.Connected;

            //If this connection was caused by a reconnect, resubscribe to all subscriptions silently
            if (_CurrentErrorState != ErrorStates.Ok)
            {
                Subscribe(_currentlySubscribed, (topics) =>
                {
                    _currentlySubscribed.ForEach((topic) =>
                    {
                        var currT = topics.Find(t => t.GetHashCode() == topic.GetHashCode());
                        if (currT != null){ return; }
                        //Topic couldn't resubscribe
                        OnTopicResubscriptionFailed?.Invoke(topic);
                        _currentlySubscribed.Remove(topic);
                    });
                }, null);
            }
            
            _CurrentErrorState = ErrorStates.Ok;
            
            OnConnected?.Invoke();
        }

        /// <summary>
        /// Internally handled disconnection and event pass through
        /// </summary>
        private void OnInternalDisconnected()
        {
            _currentState = ConnectionStates.Disconnected;
            OnDisconnected?.Invoke();
        }
        
        /// <summary>
        /// Internal Handler for connection failures
        /// Initiates retry & back-off states
        /// Note: OnConnectionError only fires if we cannot retry after 15 minutes.
        /// </summary>
        /// <param name="error"></param>
        private void OnInternalConnectionError(PlayFabError error)
        {
            _currentState = ConnectionStates.ConnectionFailed;
            switch (_CurrentErrorState)
            {
                case ErrorStates.Ok:
                    _CurrentErrorState = ErrorStates.Retry30S;
                    OnRetryConnection?.Invoke((float)_CurrentErrorState,"Trying to Reconnect in 30s");
                    if(Debugging) Debug.Log("Trying to Reconnect in 30s");
                    break;
                case ErrorStates.Retry30S:
                    _CurrentErrorState = ErrorStates.Retry5M;
                    OnRetryConnection?.Invoke((float)_CurrentErrorState,"Trying to Reconnect in 5m");
                    if(Debugging) Debug.Log("Trying to Reconnect in 5m");
                    break;
                case ErrorStates.Retry5M:
                    _CurrentErrorState = ErrorStates.Retry10M;
                    OnRetryConnection?.Invoke((float)_CurrentErrorState,"Trying to Reconnect in 10m");
                    if(Debugging) Debug.Log("Trying to Reconnect in 10m");
                    break;
                case ErrorStates.Retry10M:
                    _CurrentErrorState = ErrorStates.Retry15M;
                    OnRetryConnection?.Invoke((float)_CurrentErrorState,"Trying to Reconnect in 15m");
                    if(Debugging) Debug.Log("Trying to Reconnect in 15m");
                    break;
                case ErrorStates.Retry15M:
                    _CurrentErrorState = ErrorStates.Cancelled;
                    OnRetryConnection?.Invoke((float)_CurrentErrorState,"Cannot reconnect to server, no more retries");
                    OnConnectionError?.Invoke(error);
                    if(Debugging) Debug.Log("Cannot reconnect to server, no more retries");
                    break;
            }
        }
        
        /// <summary>
        /// Update loop to perform retry tasking
        /// </summary>
        private void LateUpdate()
        {
            if (_CurrentErrorState == ErrorStates.Cancelled) { return;}
            
            _Timer += Time.deltaTime;
        
            if (_CurrentErrorState != ErrorStates.Ok && _currentState == ConnectionStates.ConnectionFailed)
            {
                _RetryMax = (float) _CurrentErrorState + _Jitter;
                if (_Timer >= _RetryMax)
                {
                    _Jitter = new System.Random().Next(0, (int)_CurrentErrorState / 2);
                    _Timer = 0f;
                    _currentState = ConnectionStates.Connecting;
                    Connect();
                }
            }
        }
    }
}
