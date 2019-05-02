using System.Threading;
using UnityEngine;

namespace PlayFab.Sockets
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http.Connections;
    using Microsoft.AspNetCore.SignalR.Client;
    using Models;
    using Newtonsoft.Json.Linq;

    public class PlayFabSignalRService
    {
        //Delegates for events fired from the service
        public delegate void OnConnectEvent();
        public delegate void OnDisconnectEvent();
        public delegate void OnConnectionFailedEvent(PlayFabError error);

        public bool Debugging { get; set; }

        //Delegated events that you can subscribe to recieve notification from the service 
        public event OnConnectEvent OnConnect;
        public event OnDisconnectEvent OnDisconnect;
        public event OnConnectionFailedEvent OnConnectionError;

        /// <summary>
        /// Reference to Hub Connection
        /// </summary>
        private HubConnection _hubConnection = null;

        /// <summary>
        /// Access Token for making requests to Relay
        /// </summary>
        private string _accessToken = "";

        /// <summary>
        /// Url for making requests to Relay
        /// </summary>
        private string _uri = "";

        /// <summary>
        /// Dictionary of topic handlers the Client is subscribed
        /// </summary>
        private static readonly ConcurrentDictionary<Topic, ConcurrentBag<Action<PlayFabNetworkMessage>>> UserTopicHandlers = new ConcurrentDictionary<Topic, ConcurrentBag<Action<PlayFabNetworkMessage>>>();

        /// <summary>
        /// Connect to the PlayFab to get connection info
        /// </summary>
        public void Connect()
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                //Invoke Authentication error if we are not logged in and stop processing.
                OnConnectionError?.Invoke(new PlayFabError()
                {
                    Error = PlayFabErrorCode.NotAuthenticated,
                    ErrorMessage = "Developer must authenticate with a PlayFabClientAPI Login before calling Connect on the Service."
                });
                return;
            }

            //Reach out to PlayFab endpoint and get the connection info
            Internal.PlayFabHttp.MakeApiCall<GetConnectionInfoResponse>(Endpoints.GetConnectionInfo,
                new GetConnectionInfoRequest(),
                Internal.AuthType.EntityToken,
                OnConnectionInfoSuccess, OnConnectionInfoFailure);
        }

        /// <summary>
        /// Disconnect from the hub, this removes all subscriptions server side
        /// </summary>
        public void Disconnect()
        {
            InternalDisconnect();
        }

        /// <summary>
        /// This async method actually does the disconnection work
        /// </summary>
        private async void InternalDisconnect()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
            }
            OnDisconnect?.Invoke();
        }

        /// <summary>
        /// Handles connection info response from PlayFab (called on Connect) and starts the connection to the hub.
        /// </summary>
        /// <param name="connectionInfo"></param>
        private async void OnConnectionInfoSuccess(GetConnectionInfoResponse connectionInfo)
        {
            _accessToken = connectionInfo.AccessToken;
            _uri = connectionInfo.Url;
            CreateHubConnection();
            try
            {
                if (Debugging) { Debug.Log("Trying to connect to hub"); }
                await _hubConnection.StartAsync();
                if (Debugging) { Debug.Log("Connected To Hub"); }
                OnConnect?.Invoke();
            }
            catch (Exception ex)
            {
                OnConnectionError?.Invoke(new PlayFabError()
                {
                    Error = PlayFabErrorCode.InternalServerError,
                    ErrorMessage =
                        string.Format("PersistentSocket failed to start the connection with the message: {0}",
                            !string.IsNullOrEmpty(ex.Message) ? ex.Message : string.Empty)
                });
            }

        }

        /// <summary>
        /// Handler for if we could not connect to a PlayFab title and get SignalR Hub connection info
        /// </summary>
        /// <param name="error"></param>
        private void OnConnectionInfoFailure(PlayFabError error)
        {
            OnConnectionError?.Invoke(error);
        }

        /// <summary>
        /// Subscribe to a PlayStream or Message topic
        /// </summary>
        /// <param name="topic">The topic you wish to subscribe to</param>
        /// <param name="subscribeComplete">Fires if subscription was successful</param>
        /// <param name="exceptionCallback">Fires if the subscription was unsuccessful</param>
        public void Subscribe(Topic topic, Action subscribeComplete, Action<Exception> exceptionCallback, Action<PubSubServiceException> subscribeFailedCallback = null)
        {
            InternalSubscribe(topic, subscribeComplete, exceptionCallback, subscribeFailedCallback);
        }

        /// <summary>
        /// Subscribe to multiple PlayStream or Message topics at once
        /// </summary>
        /// <param name="topics">A list of topics you wish to subscribe to</param>
        /// <param name="subscribeComplete">Fires upon success, returns a List of topics that were successfully subscribed to. pass null to omit event</param>
        /// <param name="exceptionCallback">Fires upon failure to subscribe, returns a list of Exceptions from failed subscriptions. pass null to omit event</param>
        public void Subscribe(List<Topic> topics, Action<List<Topic>> subscribeComplete, Action<List<Exception>> exceptionCallback)
        {
            var taskList = new Task(() =>
            {
                var listOfExceptions = new List<Exception>();
                var listOfTopicsSuccess = new List<Topic>();
                var topicCount = topics.Count;
                var topicProgress = 0;
                topics.ForEach((topic) =>
                {
                    var t = topic;
                    Subscribe(t, () =>
                    {
                        listOfTopicsSuccess.Add(t);
                        topicProgress++;
                        if (Debugging)
                        {
                            Debug.LogFormat("Topic: {0} added", t.FullName);
                        }
                    }, (error) =>
                    {
                        listOfExceptions.Add(error);
                        topicProgress++;
                    });
                });

                while (topicProgress < topicCount)
                {
                    Thread.Sleep(1);
                }

                if (listOfTopicsSuccess.Count > 0)
                {
                    subscribeComplete?.Invoke(listOfTopicsSuccess);
                }
                if (listOfExceptions.Count > 0)
                {
                    exceptionCallback?.Invoke(listOfExceptions);
                }
            });
            taskList.Start();
        }


        /// <summary>
        /// Internal method to perform the subscription aysc
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="subscribeComplete"></param>
        /// <param name="exceptionCallback"></param>
        private async void InternalSubscribe(Topic topic, Action subscribeComplete, Action<Exception> exceptionCallback, Action<PubSubServiceException> subscribeExceptionCallback)
        {
            try
            {
                var pubSubResponse = await _hubConnection.InvokeAsync<PubSubResponse>("Subscribe", new SubscribeRequest { Topic = topic });
                if (pubSubResponse.code != 200)
                {
                    PubSubJsonError errorResponse = ((JObject)pubSubResponse.content).ToObject<PubSubJsonError>();

                    subscribeExceptionCallback?.Invoke(new PubSubServiceException(errorResponse.error, errorResponse.errorDetails["requestId"]));
                }
                else
                {
                    subscribeComplete?.Invoke();
                }
            }
            catch (Exception ex)
            {
                exceptionCallback?.Invoke(ex);
            }
        }

        /// <summary>
        /// Unsubscribe from a topic
        /// </summary>
        /// <param name="topic">The topic you wish to unsubscribe from</param>
        /// <param name="unsubscribeComplete">Fires if unsubscription was successful, pass null to omit event</param>
        /// <param name="exceptionCallback">Fires if unsubscription was not successful, pass null to omit event</param>
        public void Unsubscribe(Topic topic, Action unsubscribeComplete, Action<Exception> exceptionCallback, Action<PubSubServiceException> unsubscribeExceptionCallback = null)
        {
            InternalUnsubscribe(topic, unsubscribeComplete, exceptionCallback, unsubscribeExceptionCallback);
        }


        /// <summary>
        /// Unsubscribe from multiple topics at onces
        /// </summary>
        /// <param name="topics">List of topics to unsubscribe from</param>
        /// <param name="unsubscribeComplete">List of topics you successfully unsubscribed from. pass null to omit event</param>
        /// <param name="exceptionCallback">List of exceptions from failed unsubscriptions. pass null to omit event</param>
        public void Unsubscribe(List<Topic> topics, Action<List<Topic>> unsubscribeComplete, Action<List<Exception>> exceptionCallback)
        {
            var taskList = new Task(() =>
            {
                var listOfExceptions = new List<Exception>();
                var listOfTopicsSuccess = new List<Topic>();
                var topicCount = topics.Count;
                var topicProgress = 0;
                topics.ForEach((topic) =>
                {
                    var t = topic;
                    Unsubscribe(t, () =>
                    {
                        listOfTopicsSuccess.Add(t);
                    }, (error) =>
                    {
                        listOfExceptions.Add(error);
                    });
                });
                while (topicProgress < topicCount)
                {
                    Thread.Sleep(1);
                }
                if (listOfTopicsSuccess.Count > 0)
                {
                    unsubscribeComplete?.Invoke(listOfTopicsSuccess);
                }
                if (listOfExceptions.Count > 0)
                {
                    exceptionCallback?.Invoke(listOfExceptions);
                }
            });
            taskList.Start();
        }

        /// <summary>
        /// Internal method to perform the unsubscription asyc
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="unsubscribeComplete"></param>
        /// <param name="exceptionCallback"></param>
        private async void InternalUnsubscribe(Topic topic, Action unsubscribeComplete, Action<Exception> exceptionCallback, Action<PubSubServiceException> unsubscribeExceptionCallback)
        {
            try
            {
                var pubSubResponse = await _hubConnection.InvokeAsync<PubSubResponse>("Unsubscribe", new UnsubscribeRequest { Topic = topic });
                if (pubSubResponse.code != 200)
                {
                    PubSubJsonError errorResponse = ((JObject)pubSubResponse.content).ToObject<PubSubJsonError>();

                    unsubscribeExceptionCallback?.Invoke(new PubSubServiceException(errorResponse.error, errorResponse.errorDetails["requestId"]));
                }
                else
                {
                    unsubscribeComplete?.Invoke();
                }
            }
            catch (Exception ex)
            {
                exceptionCallback?.Invoke(ex);
            }
        }

        /// <summary>
        /// Register a handler to receive messages on a topic
        /// </summary>
        /// <param name="topic">Topic you wish to receive a message about</param>
        /// <param name="handler">Function (Action) handler that can receive the message</param>
        public void RegisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            if (UserTopicHandlers.ContainsKey(topic))
            {
                // only allow unique handlers, we don't want to double call the same function
                if (!UserTopicHandlers[topic].Contains(handler))
                {
                    UserTopicHandlers[topic].Add(handler);
                }
            }
            else
            {
                UserTopicHandlers.TryAdd(topic, new ConcurrentBag<Action<PlayFabNetworkMessage>>() { handler });
            }
        }

        /// <summary>
        /// Unregister a handler from receiving messages on a topic
        /// </summary>
        /// <param name="topic">Topic you no longer wish to handle messages for</param>
        /// <param name="handler">Original handler that you previously registered to handle the message</param>
        public void UnregisterHandler(Topic topic, Action<PlayFabNetworkMessage> handler)
        {
            if (!UserTopicHandlers.ContainsKey(topic)) return;
            var hasMultipleHandlers = UserTopicHandlers[topic].Count > 1;

            if (hasMultipleHandlers)
            {
                var remainingHandlers = new ConcurrentBag<Action<PlayFabNetworkMessage>>();
                foreach (var handle in UserTopicHandlers[topic])
                {
                    if (handle != handler)
                    {
                        remainingHandlers.Add(handle);
                    }
                }
                UserTopicHandlers.TryAdd(topic, remainingHandlers);
            }
            else
            {
                ConcurrentBag<Action<PlayFabNetworkMessage>> outBag;
                UserTopicHandlers.TryRemove(topic, out outBag);
            }
        }

        /// <summary>
        /// Create the action Hub connection and listen for messages received
        /// </summary>
        private void CreateHubConnection()
        {
            try
            {
                //Note: removed error trapping on setting strings (_uri & _accessToken) because they are guaranteed to be there.
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_uri, options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                        options.AccessTokenProvider = () => Task.FromResult(_accessToken);
                    }).Build();

                var closedTcs = new TaskCompletionSource<object>();
                _hubConnection.Closed += e =>
                {
                    if (Debugging) { UnityEngine.Debug.Log($"Connection closed: {e}"); }
                    closedTcs.SetResult(null);
                    return Task.CompletedTask;
                };

                _hubConnection.On<PlayFabNetworkMessage>("ReceiveEvent", InternalOnReceiveMessage);

                if (Debugging)
                {
                    UnityEngine.Debug.Log("Hub Created! This should only happen once");
                }

            }
            catch (Exception ex)
            {
                OnConnectionError?.Invoke(new PlayFabError()
                {
                    Error = PlayFabErrorCode.InternalServerError,
                    ErrorMessage = string.Format("PersistentSocket failed to start the connection with the message: {0}", !string.IsNullOrEmpty(ex.Message) ? ex.Message : string.Empty)
                });
            }
        }

        /// <summary>
        /// Internally Handle messages received and forward them to the Registered Handlers
        /// </summary>
        /// <param name="netMsgString"></param>
        private void InternalOnReceiveMessage(PlayFabNetworkMessage netMsg)
        {
            if(UserTopicHandlers.Keys.ToList().Find(k=>k.Equals(netMsg.@event.GetTopic())) == null) return;
            foreach (var action in UserTopicHandlers[netMsg.@event.GetTopic()])
            {
                action?.Invoke(netMsg);
            }
        }

    }
}
