using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class LeaderboardPaneController : MonoBehaviour
	{

		private const float MIN_LEADERBOARD_UPDATE_INTERVAL = 5.0f;

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

		private float lastLeaderboardUpdateTime = 0;
		private int currentStatIndex = 0;

		[System.NonSerialized] private LeaderboardManager m_leaderboardManager;

		//private string _blank = "_____________";

		public void Init(LeaderboardManager p_leaderboardManager)
		{
			m_leaderboardManager = p_leaderboardManager;
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

			//var stat = this.PlayerLevelStats.Find(val => val == "Total_AdsWatched");
			//      if (string.IsNullOrEmpty(stat))
			string stat = PlayerLevelStats[currentStatIndex];
			StatisticFilter.GetComponentInChildren<Text>().text = stat;
			UpdateLeaderboard(stat);
			UpdateFriendList();
		}

		private void OnEnable()
		{
			TelemetryManager.RecordScreenViewed(TelemetryScreenId.Leaders);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		public void CategoryFilterClicked()
		{
			UnityAction<int> afterSelect = (int response) =>
			{
				if (response == 0) // stand alone
				{
					CategoryFilter.GetComponentInChildren<Text>().text = "Standalone";
					this.ActiveOptions = this.StandAloneStatistics;

					this.QuestContainer.gameObject.SetActive(false);
					this.StatsContainer.anchoredPosition = new Vector2(this.QuestContainer.anchoredPosition.x + 5, this.QuestContainer.anchoredPosition.y);
				}
				else if (response == 1) // per quest
				{
					CategoryFilter.GetComponentInChildren<Text>().text = "Per Quest";
					this.ActiveOptions = this.QuestStatistics;

					this.StatsContainer.anchoredPosition = new Vector2(this.StatsContainer.anchoredPosition.x + this.StatsContainer.rect.width, this.StatsContainer.anchoredPosition.y);
					this.QuestContainer.gameObject.SetActive(true);
				}
			};

			string[] categories = new string[] { "Standalone", "Per Quest" };
			DialogCanvasController.RequestSelectorPrompt(GlobalStrings.CATEGORY_SELECTOR_PROMPT, categories, afterSelect);
		}

		public void StatisticFilterClicked()
		{
			UnityAction<int> afterSelect = (int response) =>
			{
				currentStatIndex = response;
				StartCoroutine(UpdateLeaderboard(PlayerLevelStats[response]));
				//UpdateMyRank(this.PlayerLevelStats[response]);
			};

			DialogCanvasController.RequestSelectorPrompt(GlobalStrings.STAT_SELECTOR_PROMPT, this.PlayerLevelStats.ToArray(), afterSelect);
		}

		public void QuestFilterClicked()
		{

			UnityAction<int> afterSelect = (int response) => { };

			var l_gameDataManager = MainManager.Instance.getGameDataManager();
			if (null == l_gameDataManager) return;

			l_gameDataManager.Refresh(false, (s) =>
			{
				Quests = new List<string>(l_gameDataManager.GetAllLevelNames());
				if (Quests.Count == 0) return;

				DialogCanvasController.RequestSelectorPrompt(GlobalStrings.QUEST_SELECTOR_PROMPT, Quests.ToArray(), afterSelect);
			});

		}

		public void AddFriendClicked()
		{
			UnityAction<int> afterSelect = (int response) =>
			{
				Action<string> afterInput = (string input) =>
				{
					if (string.IsNullOrEmpty(input)) return;
					DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.AddFriend);

					FriendsManager l_friendsMgr = MainManager.Instance.getFriendsManager();
					if (null == l_friendsMgr) return;

					l_friendsMgr.AddFriend(
						input,
						(FriendsManager.AddFriendMethod) response,
						() =>
						{
							// no real data to be sent with this event, just sending an empty dict for now...
							TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_FriendAdded, new Dictionary<string, object>());

							// force refresh
							l_friendsMgr.Refresh(true,
								(s) =>
								{
									UpdateFriendList();
									PF_Bridge.RaiseCallbackSuccess(s, PlayFabAPIMethods.GetFriendList);
								},
								(e) =>
								{
									PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetFriendList);
								}
							);
							PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.AddFriend);
						},
						(e) =>
						{
							PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.AddFriend);
						}
					);
				};

				DialogCanvasController.RequestTextInputPrompt(GlobalStrings.ADD_FRIEND_PROMPT, string.Format(GlobalStrings.ADD_FRIEND_MSG, (FriendsManager.AddFriendMethod) response), afterInput);

			};
			DialogCanvasController.RequestSelectorPrompt(GlobalStrings.FRIEND_SELECTOR_PROMPT, Enum.GetNames(typeof(FriendsManager.AddFriendMethod)).ToArray(), afterSelect);

		}

		public void UpdateMyRank(string stat)
		{
			//PF_GameData.GetMyCharacterLeaderboardRank(stat, afterRank);
			m_leaderboardManager.RefreshMyPlayerLeaderboardRank(
				stat,
				(int rank) =>
				{
					this.myPosition.text = rank > 0 ? "" + rank : "--";
					PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetMyPlayerRank);
				},
				(f) => { PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.GetMyPlayerRank); }
			);
		}

		private IEnumerator UpdateLeaderboard(string stat)
		{
			var now = Time.realtimeSinceStartup;
			var next = lastLeaderboardUpdateTime + MIN_LEADERBOARD_UPDATE_INTERVAL;
			var diff = next - now;
			StatisticFilter.GetComponentInChildren<Text>().text = stat;
			if (diff > 0)
			{
				DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetFriendsLeaderboard);
				yield return new WaitForSeconds(diff);
			}

			RefreshLeaderboards(stat);
			lastLeaderboardUpdateTime = now;
			yield return null;
		}

		public void RefreshLeaderboards(string stat)
		{
			Action<string> afterGetLB = (string result) =>
			{
				Debug.Log("Update Top10 LB UI");
				//if (PF_GameData.currentTop10LB.Count <= 0) return;

				var l_CurrentTopTenList = m_leaderboardManager.GetPlayerLeaderboard();
				if (null != l_CurrentTopTenList && l_CurrentTopTenList.Length > 0)
				{
					int count = 0;
					foreach (var rank in l_CurrentTopTenList)
					{
						var item = this.Top10LB.items[count];
						item.Rank.text = "" + (rank.Position + 1);
						item.Name.text = string.IsNullOrEmpty(rank.DisplayName) ? rank.PlayFabId : rank.DisplayName;
						item.Value.text = "" + rank.StatValue;
						count++;
					}
					if (count < 10)
					{
						for (int z = count; z < 10; z++)
						{
							var item = this.Top10LB.items[z];
							item.Rank.text = "" + (z + 1);
							item.Name.text = string.Empty;
							item.Value.text = string.Empty;
						}
					}
				}
				var l_CurrentFriendsList = m_leaderboardManager.GetFriendsLeaderboard();
				if (null != l_CurrentFriendsList && l_CurrentFriendsList.Length > 0)
				{
					int count = 0;
					foreach (var rank in l_CurrentFriendsList)
					{
						var item = this.FriendsLB.items[count];
						item.Rank.text = "" + (rank.Position + 1);
						item.Name.text = string.IsNullOrEmpty(rank.DisplayName) ? rank.PlayFabId : rank.DisplayName;
						item.Value.text = "" + rank.StatValue;
						count++;
					}
					if (count < 10)
					{
						for (int z = count; z < 10; z++)
						{
							var item = this.FriendsLB.items[z];
							item.Rank.text = "" + (z + 1);
							item.Name.text = string.Empty;
							item.Value.text = string.Empty;
						}
					}
				}
				PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetPlayerLeaderboard);
				PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetFriendsLeaderboard);
			};

			m_leaderboardManager.RefreshLeaderboards(stat, afterGetLB,
				(f) =>
				{
					PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.GetPlayerLeaderboard);
					PF_Bridge.RaiseCallbackError(f, PlayFabAPIMethods.GetFriendsLeaderboard);
				}
			);

		}

		public void UpdateFriendList()
		{
			FriendsManager l_friendsMgr = MainManager.Instance.getFriendsManager();
			if (null == l_friendsMgr) return;

			UnityAction afterGetFriends = () =>
			{
				Debug.Log("Update Friend List UI");

				LayoutElement[] children = this.friendListView.GetComponentsInChildren<LayoutElement>();
				for (int z = 0; z < children.Length; z++)
				{
					if (children[z].gameObject != this.friendListView.gameObject)
					{
						DestroyImmediate(children[z].gameObject);

					}
				}

				foreach (var friend in l_friendsMgr.GetFriendsArray())
				{
					Transform item = Instantiate(this.friendItemPrefab);

					Text txt = item.GetComponentInChildren<Text>();
					txt.text = friend.TitleDisplayName;
					//string id = friend.FriendPlayFabId;

					Button btn = item.GetComponent<Button>();

					UBFriendInfo friendCaptured = friend;
					btn.onClick.RemoveAllListeners();
					btn.onClick.AddListener(() =>
					{
						FriendClicked(friendCaptured);
					});

					item.SetParent(this.friendListView, false);

				}
			};

			l_friendsMgr.Refresh(false,
				(s) =>
				{
					afterGetFriends();
					PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetFriendList);
				},
				(e) =>
				{
					PF_Bridge.RaiseCallbackError(e, PlayFabAPIMethods.GetFriendList);
				}
			);
		}

		public void FriendClicked(UBFriendInfo friend)
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
}