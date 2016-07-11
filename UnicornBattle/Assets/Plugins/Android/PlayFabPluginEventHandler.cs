using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayFab.Internal
{
    public class PlayFabPluginEventHandler : MonoBehaviour
    {
        private static PlayFabPluginEventHandler _playFabEvtHandler;

        public static void Init()
        {
            //Check if we have already created this object.
            if (_playFabEvtHandler != null)
            {
                Debug.Log("_PlayFabGO object already created.");
                return;
            }

            //Create GameObject and place it in the scene.
            var playfabGo = GameObject.Find("_PlayFabGO");
            if (playfabGo == null)
            {
                Debug.LogFormat("Createding new _PLayFabGO");
                playfabGo = new GameObject("_PlayFabGO");
                DontDestroyOnLoad(playfabGo);
            }

            //If the event handler component exists
            _playFabEvtHandler = playfabGo.GetComponent<PlayFabPluginEventHandler>() ??
                                 playfabGo.AddComponent<PlayFabPluginEventHandler>();
        }

        public void GCMRegistrationReady(string status)
        {
            bool statusParam;
            bool.TryParse(status, out statusParam);
            PlayFabGoogleCloudMessaging.RegistrationReady(statusParam);
        }

        public void GCMRegistered(string token)
        {
            var error = (string.IsNullOrEmpty(token)) ? token : null;
            PlayFabGoogleCloudMessaging.RegistrationComplete(token, error);
        }

        public void GCMRegisterError(string error)
        {
            PlayFabGoogleCloudMessaging.RegistrationComplete(null, error);
        }

        public void GCMMessageReceived(string message)
        {
            PlayFabGoogleCloudMessaging.MessageReceived(message);
        }
    }
}