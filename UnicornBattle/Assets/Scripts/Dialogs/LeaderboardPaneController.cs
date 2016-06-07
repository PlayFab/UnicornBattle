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


public class LeaderboardPaneController : MonoBehaviour {
	public Button CategoryFilter;
	public Button StatisticFilter;
	public Button QuestFilter;
	
	public RectTransform CategoryContainer;
	public RectTransform QuestContainer;
	public RectTransform StatsContainer;


	public List<string> Quests = new List<string>();
	public List<string> StandAloneStatistics = new List<string>();
	public List<string> QuestStatistics = new List<string>();
	public List<string> ActiveOptions = new List<string>();
	public List<string> PlayerLevelStats = new List<string>();
	
	public LeaderboardController Top10LB;
	public LeaderboardController FriendsLB;

	public FriendListController FriendListController;
	public FriendDetailsController detailsController;
	
	public Transform friendItemPrefab;
	public Transform friendListView;

	public Text myPosition;

	//private string _blank = "_____________";

	public void Init()
	{
		//select default stat,
		// get top10 for stat
		// get mypos for stat
		// get friends
		
		// get other leaderboard?
//		CategoryFilter.GetComponentInChildren<Text>().text = "Standalone";
//		this.ActiveOptions = this.StandAloneStatistics;
//		
//		this.QuestContainer.gameObject.SetActive(false);
//		this.StatsContainer.anchoredPosition = new Vector2(this.QuestContainer.anchoredPosition.x + 5, this.QuestContainer.anchoredPosition.y);
//		this.StatisticFilter.GetComponentInChildren<Text>().text = this.StandAloneStatistics[0];
//		
		// this is all fairly hacky, and should be smoothed over.
		UpdateTop10LB(this.PlayerLevelStats[0]);
		UpdateFriendsLB(this.PlayerLevelStats[0]);
		
		UpdateFriendList();
		//UpdateMyRank(this.PlayerLevelStats[0]);
		
		this.StatisticFilter.GetComponentInChildren<Text>().text = this.PlayerLevelStats[0];
	}
	

	public void CategoryFilterClicked()
	{
		UnityAction<int> afterSelect = (int response)=>
		{
			if( response == 0) // stand alone
			{
				CategoryFilter.GetComponentInChildren<Text>().text = "Standalone";
				this.ActiveOptions = this.StandAloneStatistics;
				
				this.QuestContainer.gameObject.SetActive(false);
				this.StatsContainer.anchoredPosition = new Vector2(this.QuestContainer.anchoredPosition.x + 5, this.QuestContainer.anchoredPosition.y);
			}
			else if( response == 1) // per quest
			{
				CategoryFilter.GetComponentInChildren<Text>().text = "Per Quest";
				this.ActiveOptions = this.QuestStatistics;
				
				this.StatsContainer.anchoredPosition = new Vector2(this.StatsContainer.anchoredPosition.x + this.StatsContainer.rect.width, this.StatsContainer.anchoredPosition.y);
				this.QuestContainer.gameObject.SetActive(true);
			}
		};

		List<string> categories = new List<string>() {"Standalone", "Per Quest" };
        DialogCanvasController.RequestSelectorPrompt(GlobalStrings.CATEGORY_SELECTOR_PROMPT, categories, afterSelect);
	}


	public void StatisticFilterClicked()
	{
		UnityAction<int> afterSelect = (int response)=>
		{
			this.StatisticFilter.GetComponentInChildren<Text>().text = this.PlayerLevelStats[response];
			UpdateFriendsLB(this.PlayerLevelStats[response]);
			UpdateTop10LB(this.PlayerLevelStats[response]);
			//UpdateMyRank(this.PlayerLevelStats[response]);
		};

        DialogCanvasController.RequestSelectorPrompt(GlobalStrings.STAT_SELECTOR_PROMPT, this.PlayerLevelStats, afterSelect);
	}

	public void QuestFilterClicked()
	{
			
		UnityAction<int> afterSelect = (int response)=>
		{
			
		};
		
		List<string> quests = new List<string>();
		if(PF_GameData.Levels.Count > 0)
		{
			foreach(var quest in PF_GameData.Levels)
			{
				this.Quests.Add(quest.Key);
			}
		}
		else
		{
			Debug.Log("No quests found...");
			return;
		}

        DialogCanvasController.RequestSelectorPrompt(GlobalStrings.QUEST_SELECTOR_PROMPT, quests, afterSelect);
			
			

	}
	
	public void AddFriendClicked()
	{
		UnityAction<int> afterSelect = (int response) =>
		{
			Action<string> afterInput = (string input) =>
			{
				PF_PlayerData.AddFriend(input, (PF_PlayerData.AddFriendMethod) response, (bool result) =>
				{
					if(result == true)
					{
						Dictionary<string, object> eventData = new Dictionary<string, object>();
						// no real data to be sent with this event, just sending an empty dict for now...
						PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_FriendAdded, eventData);
						
						PF_PlayerData.GetFriendsList(() => { UpdateFriendList(); });
					}
				});
			};

            DialogCanvasController.RequestTextInputPrompt(GlobalStrings.ADD_FRIEND_PROMPT, string.Format(GlobalStrings.ADD_FRIEND_MSG, (PF_PlayerData.AddFriendMethod)response), afterInput);

        };
        DialogCanvasController.RequestSelectorPrompt(GlobalStrings.FRIEND_SELECTOR_PROMPT, Enum.GetNames(typeof(PF_PlayerData.AddFriendMethod)).ToList(), afterSelect);
		
	}

	
	public void UpdateMyRank(string stat)
	{
		UnityAction<int> afterRank = (int rank) => 
		{
			this.myPosition.text =  rank > 0 ? "" + rank : "--";
		};
		
		
		//PF_GameData.GetMyCharacterLeaderboardRank(stat, afterRank);
		PF_GameData.GetMyPlayerLeaderboardRank(stat, afterRank);
	}



	public void UpdateTop10LB(string stat)
	{
		UnityAction afterGetLB= () => 
		{
			Debug.Log ("Update Top10 LB UI");
			if(PF_GameData.currentTop10LB.Count > 0)
			{
				int count = 0;
				foreach(var rank in PF_GameData.currentTop10LB)
				{
					this.Top10LB.items[count].Rank.text = "" + (rank.Position +1);
					this.Top10LB.items[count].Name.text = string.IsNullOrEmpty(rank.DisplayName) ? rank.PlayFabId : rank.DisplayName;
					this.Top10LB.items[count].Value.text = "" + rank.StatValue;
					count++;
				}
				
				if(count < 10)
				{
					for(int z = count; z < 10; z++)
					{
						this.Top10LB.items[z].Rank.text = "" + (z+1);
						this.Top10LB.items[z].Name.text = string.Empty;
						this.Top10LB.items[z].Value.text = string.Empty;
					}
				}
			}
		}; 
		
		PF_GameData.GetPlayerLeaderboard(stat, afterGetLB);

	}
	
	public void UpdateFriendsLB(string stat)
	{
		Debug.Log ("Update Friend LB UI");
		
		UnityAction afterGetLB= () => 
		{
			if(PF_GameData.friendsLB.Count > 0)
			{
				int count = 0;
				foreach(var rank in PF_GameData.friendsLB)
				{
					this.FriendsLB.items[count].Rank.text = "" + (rank.Position +1);
					this.FriendsLB.items[count].Name.text = string.IsNullOrEmpty(rank.DisplayName) ? rank.PlayFabId : rank.DisplayName;
					this.FriendsLB.items[count].Value.text = "" + rank.StatValue;
					count++;
				}
				
				if(count < 10)
				{
					for(int z = count; z < 10; z++)
					{
						this.FriendsLB.items[z].Rank.text = "" + (z+1);
						this.FriendsLB.items[z].Name.text = string.Empty;
						this.FriendsLB.items[z].Value.text = string.Empty;
					}
				}
			}
		}; 
		
		//PF_GameData.GetCharacterLeaderboard(stat, afterGetLB); // not using this based on the character LB issues.
		PF_GameData.GetFriendsLeaderboard(stat, afterGetLB);
	}


	public void UpdateFriendList()
	{
		UnityAction afterGetFriends = () =>
		{
			Debug.Log ("Update Friend List UI");
			
			LayoutElement[] children = this.friendListView.GetComponentsInChildren<LayoutElement> ();
			for (int z = 0; z < children.Length; z++) 
			{
				if (children [z].gameObject != this.friendListView.gameObject) 
				{
					DestroyImmediate(children [z].gameObject);
					
				}
			}
			
			foreach(var friend in PF_PlayerData.playerFriends)
			{
				Transform item = Instantiate(this.friendItemPrefab);
				
				Text txt = item.GetComponentInChildren<Text>();
				txt.text = friend.TitleDisplayName;
                //string id = friend.FriendPlayFabId;
				
				Button btn = item.GetComponent<Button>();
				
				FriendInfo friendCaptured = friend;
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => 
				                        {
					FriendClicked(friendCaptured);
				});
				
				item.SetParent(this.friendListView, false);
				
			}
		};
		
		PF_PlayerData.GetFriendsList(afterGetFriends);		
	}
	
	
	
	
	public void FriendClicked(FriendInfo friend)
	{
		Debug.Log(friend.FriendPlayFabId);
		this.detailsController.gameObject.SetActive(true);
		this.detailsController.Init(friend);
		
	}



	public void CloseSocialPrompt()
	{
		this.gameObject.SetActive(false);
	}
}
