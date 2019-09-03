//using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

    public class FriendDetailsController : MonoBehaviour
    {
        public Image ProfilePhoto;
        public Transform ListView;
        public Text UserName;
        public Text PlayFabID;
        public Text Origin;
        public Text DisplayName;
        public Transform ListItemPrefab;
        public Button RemoveFriendBtn;

        public LeaderboardPaneController controller;
        public UBFriendInfo activeFriend;

        private bool _didCustomTagsChange = false;

        private FriendsManager m_friendsManager;

        public void Init(UBFriendInfo friend)
        {
            m_friendsManager = MainManager.Instance.getFriendsManager();
            if (null == m_friendsManager) return;

            _didCustomTagsChange = false;
            ClearAllTagUI();

            DisplayName.text = string.IsNullOrEmpty(friend.TitleDisplayName) ? friend.Username : friend.TitleDisplayName;
            UserName.text = string.IsNullOrEmpty(friend.Username) ? "________" : friend.Username;
            PlayFabID.text = friend.FriendPlayFabId;
            Origin.text = friend.FacebookInfo != null ? "Facebook" : "PlayFab";
            RemoveFriendBtn.interactable = friend.FacebookInfo == null;

            activeFriend = friend;
            if (friend.Tags != null)
                for (var z = 0; z < friend.Tags.Count; z++)
                    AddTagItemUI(tag, z);

            if (friend.FacebookInfo != null && !string.IsNullOrEmpty(friend.FacebookInfo.FacebookId))
            {
                UnityAction<Texture2D> afterGetPhoto = tx =>
                {
                    ProfilePhoto.overrideSprite = Sprite.Create(tx, new Rect(0, 0, 128, 128), new Vector2());
                };
#if UNITY_ANDROID || UNITY_IOS
                StartCoroutine(FacebookHelperClass.GetFriendProfilePhoto(friend.FacebookInfo.FacebookId, FetchWebAsset, afterGetPhoto));
#endif
            }
            else
            {
                ProfilePhoto.overrideSprite = null;
            }
        }

        public void OnAddCustomTagClicked()
        {
            Action<string> afterInput = response =>
            {
                if (string.IsNullOrEmpty(response))
                    return;

                if (activeFriend.Tags == null)
                    activeFriend.Tags = new List<string>();

                AddTagItemUI(response, activeFriend.Tags.Count); // since we are adding 1 immediately .Count works fine
                activeFriend.Tags.Add(response);

                _didCustomTagsChange = true;
            };
            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.FRIEND_TAG_PROMPT, GlobalStrings.FRIEND_TAG_MSG, afterInput);
        }

        public void AddTagItemUI(string tagText, int index)
        {
            var item = Instantiate(ListItemPrefab);
            item.GetComponentInChildren<Text>().text = tagText;
            item.SetParent(ListView, false);

            var btn = item.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                OnRemoveCustomTagClicked(index);
            });
        }

        public void ClearAllTagUI()
        {
            foreach (Transform item in ListView)
                if (item != transform)
                    Destroy(item.gameObject);
        }

        public void OnRemoveCustomTagClicked(int index)
        {
            activeFriend.Tags.RemoveAt(index);

            for (var z = 0; z < ListView.childCount; z++)
                if (z == index)
                    Destroy(ListView.GetChild(index).gameObject);
            _didCustomTagsChange = true;
        }

        public void OnRemoveFriendClicked()
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RemoveFriend);

            m_friendsManager.RemoveFriend(
                activeFriend.FriendPlayFabId,
                () =>
                {
                    m_friendsManager.Refresh(true, (s) =>
                        {
                            controller.UpdateFriendList();
                            OnCloseClicked();
                            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.RemoveFriend);
                        },
                        (e2) =>
                        {
                            PF_Bridge.RaiseCallbackError(e2, PlayFabAPIMethods.RemoveFriend);
                        });
                },
                (e) =>
                {
                    PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.RemoveFriend);
                }
            );
        }

        public void OnCloseClicked()
        {
            if (_didCustomTagsChange)
            {
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.SetFriendTags);

                m_friendsManager.SetFriendTags(activeFriend.FriendPlayFabId, activeFriend.Tags,
                    () =>
                    {
                        gameObject.SetActive(false);
                        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.SetFriendTags);
                    }, (e) =>
                    {
                        PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.SetFriendTags);
                    }
                );
            }

            ProfilePhoto.overrideSprite = null;
            gameObject.SetActive(false);
        }

        public void FetchWebAsset(string url, UnityAction<Texture2D> callback = null)
        {
            StartCoroutine(WebSpinner(url, callback));
        }

        public IEnumerator WebSpinner(string url, UnityAction<Texture2D> callback = null)
        {
            var uwr = new UnityWebRequest(url);
            uwr.downloadHandler = new DownloadHandlerTexture();
            yield return uwr.SendWebRequest();

            if (callback != null)
                callback(DownloadHandlerTexture.GetContent(uwr));
        }
    }
}