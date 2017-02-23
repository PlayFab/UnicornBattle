using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public FriendInfo activeFriend;

    private bool _didCustomTagsChange = false;

    public void Init(FriendInfo friend)
    {
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

            StartCoroutine(FacebookHelperClass.GetFriendProfilePhoto(friend.FacebookInfo.FacebookId, FetchWebAsset, afterGetPhoto));
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

            AddTagItemUI(response, activeFriend.Tags.Count);   // since we are adding 1 immediately .Count works fine
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
        UnityAction afterRemove = () =>
        {
            controller.UpdateFriendList();
            OnCloseClicked();
        };

        PF_PlayerData.RemoveFriend(activeFriend.FriendPlayFabId, afterRemove);
    }

    public void OnCloseClicked()
    {
        if (_didCustomTagsChange)
        {
            UnityAction afterSetTags = () =>
            {
                gameObject.SetActive(false);
            };
            PF_PlayerData.SetFriendTags(activeFriend.FriendPlayFabId, activeFriend.Tags, afterSetTags);
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
        var www = new WWW(url);
        yield return www;

        if (callback != null)
            callback(www.texture);
    }
}
