using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;


public class FriendDetailsController : MonoBehaviour {
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
	
	
	private bool didCustomTagsChange = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Init(FriendInfo friend)
	{
		// reset from previous
		this.didCustomTagsChange = false;
		ClearAllTagUI();
		
		//Update new info
		this.DisplayName.text = friend.TitleDisplayName;
		this.UserName.text = string.IsNullOrEmpty(friend.Username) ? "________" : friend.Username;
		this.PlayFabID.text = friend.FriendPlayFabId;
		this.Origin.text = friend.FacebookInfo != null ? "Facebook" : "PlayFab";
		
		if(friend.FacebookInfo != null)
		{
			this.RemoveFriendBtn.interactable = false;
		}
		else
		{
			this.RemoveFriendBtn.interactable = true;
		}
		
		this.activeFriend = friend;
		if(friend.Tags != null)
		{
			for(int z = 0; z < friend.Tags.Count; z++)
			{	
				AddTagItemUI(tag, z);
			}
		}
		
		//friend.FacebookInfo != null
		// get profile pic
		if(friend.FacebookInfo != null && !string.IsNullOrEmpty(friend.FacebookInfo.FacebookId))
		{
			UnityAction<Texture2D> afterGetPhoto = (Texture2D tx) =>
			{
				ProfilePhoto.overrideSprite = Sprite.Create(tx, new Rect(0,0, 128, 128), new Vector2()); 
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
		Action<string> afterInput = (string response) =>
		{
			if(!string.IsNullOrEmpty(response))
			{
				if(this.activeFriend.Tags == null)
				{
					this.activeFriend.Tags = new List<string>();
				}
				
				AddTagItemUI(response, this.activeFriend.Tags.Count);	// since we are adding 1 immediately .Count works fine
				this.activeFriend.Tags.Add(response);
				
				this.didCustomTagsChange = true;
			}
		};
        DialogCanvasController.RequestTextInputPrompt(GlobalStrings.FRIEND_TAG_PROMPT, GlobalStrings.FRIEND_TAG_MSG, afterInput);
	}
	
	
	public void AddTagItemUI(string tagText, int index)
	{	
		Transform item = Instantiate(this.ListItemPrefab);
		item.GetComponentInChildren<Text>().text = tagText;
		item.SetParent(this.ListView, false);
		
		Button btn = item.GetComponentInChildren<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => 
		{
			this.OnRemoveCustomTagClicked(index);
		});
		
	}
	
	public void ClearAllTagUI()
	{
		foreach(Transform item in this.ListView)
		{
			if(item != this.transform)
			{
				Destroy(item.gameObject);
			}
		}
	}
	
	
	public void OnRemoveCustomTagClicked(int index)
	{

		this.activeFriend.Tags.RemoveAt(index);
		
		for(int z = 0; z < this.ListView.childCount; z++)
		{
			if(z == index)
			{
				Destroy(this.ListView.GetChild(index).gameObject);
			}
		}
		this.didCustomTagsChange = true;
		
	}
	
	
	public void OnRemoveFriendClicked()
	{
		UnityAction afterRemove = () =>
		{
			controller.UpdateFriendList ();
			OnCloseClicked();

		};
		
		PF_PlayerData.RemoveFriend(this.activeFriend.FriendPlayFabId, afterRemove);
	}
	
	public void OnCloseClicked()
	{
		if(this.didCustomTagsChange == true)
		{
			UnityAction afterSetTags = () => 
			{
				this.gameObject.SetActive(false);	
			};
			PF_PlayerData.SetFriendTags(this.activeFriend.FriendPlayFabId, this.activeFriend.Tags, afterSetTags);
		}
		
		ProfilePhoto.overrideSprite = null;
		this.gameObject.SetActive(false);	
	}
	
	// used to get the FB picture. Probably a better way to do this.
	public void FetchWebAsset(string url, UnityAction<Texture2D> callback = null)
	{
		StartCoroutine (WebSpinner (url, callback));
	}
	
	// used to get the FB picture. Probably a better way to do this.
	public IEnumerator WebSpinner(string url, UnityAction<Texture2D> callback = null)
	{
		WWW www = new WWW(url);
		yield return www;
		
		if (callback != null) 
		{
			callback (www.texture);
		}
	}

}
